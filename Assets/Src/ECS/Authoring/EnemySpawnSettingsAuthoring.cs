using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class EnemySpawnSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnRadius;
        [SerializeField] private float spawnInterval;
        [SerializeField] private int maxEnemyCount;
        
        public class Baker : Baker<EnemySpawnSettingsAuthoring>
        {
            public override void Bake(EnemySpawnSettingsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EnemySpawnSettings
                {
                    Prefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                    SpawnRadius = authoring.spawnRadius,
                    SpawnInterval = authoring.spawnInterval,
                    NextSpawnTime = 0f,
                    MaxEnemyCount = authoring.maxEnemyCount,
                    EnemyCount = 0
                });
            }
        }
    }
    
    public struct EnemySpawnSettings : IComponentData
    {
        public Entity Prefab;
        public float SpawnRadius;
        public float SpawnInterval;
        public float NextSpawnTime;
        public int MaxEnemyCount;
        public int EnemyCount;
    }
}