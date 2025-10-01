using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class HealthAuthoring : MonoBehaviour
    {
        [SerializeField] private int value;
        
        public class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health
                {
                    Value = authoring.value,
                });
            }
        }
    }

    public struct Health : IComponentData
    {
        public int Value;
    }
}