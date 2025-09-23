using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TestRPG.ECS
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnRadius;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float spawnInterval;
        [SerializeField] private float enemySpeed;

        public class Baker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EnemySpawnerComponent
                {
                    Prefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                    SpawnCenter = authoring.transform.position,
                    SpawnRadius = authoring.spawnRadius,
                    TargetPosition = authoring.targetPosition,
                    SpawnInterval = authoring.spawnInterval,
                    NextSpawnTime = 0f,
                    EnemySpeed = authoring.enemySpeed
                });
            }
        }
    }
    
    public struct EnemySpawnerComponent : IComponentData
    {
        public Entity Prefab;
        public float3 SpawnCenter;
        public float SpawnRadius;
        public float3 TargetPosition;
        public float SpawnInterval;
        public float NextSpawnTime;
        public float EnemySpeed;
    }
}