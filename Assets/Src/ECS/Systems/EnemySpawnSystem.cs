using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemySpawnSystem : ISystem
    {
        private Random random;
        private Entity playerEntity;
        
        private Entity enemySpawnSettingsEntity;
        private bool isInit;
        private PhysicsMass prefabMass;
        private FollowToTarget prefabFollowToTarget;
        private bool isPrefabInit;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            random = new Random((uint)SystemAPI.Time.ElapsedTime + 1);
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<EnemySpawnSettings>();
            state.RequireForUpdate<Player>();

            isInit = false;
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!isInit)
            {
                isInit = true;
                playerEntity = SystemAPI.GetSingletonEntity<Player>();
                enemySpawnSettingsEntity = SystemAPI.GetSingletonEntity<EnemySpawnSettings>();
            }
            
            var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);
            float deltaTime = SystemAPI.Time.DeltaTime;
            var spawnSettings = SystemAPI.GetComponentRW<EnemySpawnSettings>(enemySpawnSettingsEntity);
            var spawnerData = spawnSettings.ValueRO;
            spawnSettings.ValueRW.NextSpawnTime -= deltaTime;

            if (!isPrefabInit)
            {
                isPrefabInit = true;
                prefabMass = state.EntityManager.GetComponentData<PhysicsMass>(spawnSettings.ValueRO.Prefab);
                prefabMass.InverseInertia = float3.zero;
                
                prefabFollowToTarget = state.EntityManager.GetComponentData<FollowToTarget>(spawnSettings.ValueRO.Prefab);
                prefabFollowToTarget.Target = playerEntity;
            }
            
            if (spawnerData.NextSpawnTime <= 0f)
            {
                SpawnEnemy(ref state, spawnerData, playerTransform.ValueRO.Position);
                spawnSettings.ValueRW.NextSpawnTime = spawnerData.SpawnInterval;
            }
        }
        
        private void SpawnEnemy(ref SystemState state, EnemySpawnSettings settings, float3 centerPosition)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            Entity enemy = ecb.Instantiate(settings.Prefab);
            
            float2 randomCircle = random.NextFloat2Direction();
            float3 spawnPosition = centerPosition + 
                                   new float3(randomCircle.x, 0, randomCircle.y) * settings.SpawnRadius;
            
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            ecb.SetComponent(enemy, prefabMass);
            ecb.SetComponent(enemy, prefabFollowToTarget);
        }
    }
}