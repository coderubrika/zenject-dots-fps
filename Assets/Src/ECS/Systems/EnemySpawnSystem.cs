using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemySpawnSystem : ISystem
    {
        private Random random;
        private Entity playerEntity;
        
        private Entity enemySpawnSettingsEntity;
        private bool isInit;
        private PhysicsMass mass;
        private FollowToTarget followToTarget;
        private AttackTarget attackTarget;
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
                mass = state.EntityManager.GetComponentData<PhysicsMass>(spawnSettings.ValueRO.Prefab);
                mass.InverseInertia = float3.zero;
                
                followToTarget = state.EntityManager.GetComponentData<FollowToTarget>(spawnSettings.ValueRO.Prefab);
                followToTarget.Target = playerEntity;

                attackTarget = state.EntityManager.GetComponentData<AttackTarget>(spawnSettings.ValueRO.Prefab);
                attackTarget.Target = playerEntity;
            }
            
            if (spawnerData.NextSpawnTime <= 0f)
            {
                if (spawnSettings.ValueRO.EnemyCount < spawnSettings.ValueRO.MaxEnemyCount)
                {
                    SpawnEnemy(ref state, spawnerData, playerTransform.ValueRO.Position);
                    spawnSettings.ValueRW.EnemyCount += 1;
                }
                
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
            
            ecb.SetComponent(enemy, mass);
            ecb.SetComponent(enemy, followToTarget);
            ecb.SetComponent(enemy, attackTarget);
        }
    }
}