using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Enemy());
            }
        }
    }
    
    public struct Enemy : IComponentData {}
}