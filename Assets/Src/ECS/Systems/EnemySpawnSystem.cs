using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemySpawnSystem : ISystem
    {
        private Random random;
        
        public void OnCreate(ref SystemState state)
        {
            random = new Random((uint)SystemAPI.Time.ElapsedTime + 1);
            state.RequireForUpdate<EnemySpawnerComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
        
            foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
            {
                var spawnerData = spawner.ValueRO;
            
                // Уменьшаем таймер
                spawner.ValueRW.NextSpawnTime -= deltaTime;
            
                if (spawnerData.NextSpawnTime <= 0f)
                {
                    SpawnEnemy(ref state, spawnerData);
                
                    // Сбрасываем таймер
                    spawner.ValueRW.NextSpawnTime = spawnerData.SpawnInterval;
                }
            }
        }
        
        private void SpawnEnemy(ref SystemState state, EnemySpawnerComponent spawner)
        {
            float2 randomCircle = random.NextFloat2Direction();
            float3 spawnPosition = spawner.SpawnCenter + 
                                   new float3(randomCircle.x, 0, randomCircle.y) * spawner.SpawnRadius;
            
            Entity enemy = state.EntityManager.Instantiate(spawner.Prefab);
        
            // Устанавливаем позицию
            state.EntityManager.SetComponentData(enemy, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            });
        
            // Добавляем компонент движения к цели
            // state.EntityManager.AddComponentData(enemy, new MoveToTarget
            // {
            //     TargetPosition = spawner.TargetPosition,
            //     Speed = spawner.EnemySpeed
            // });
            
            //state.EntityManager.AddComponent<EnemyTag>(enemy);
        }
    }
}