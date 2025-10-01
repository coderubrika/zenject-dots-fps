using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class DamageAuthoring : MonoBehaviour
    {
        [SerializeField] private int value;

        public class Baker : Baker<DamageAuthoring>
        {
            public override void Bake(DamageAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Damage
                {
                    Value = authoring.value
                });
            }
        }
    }

    public struct Damage : IComponentData
    {
        public int Value;
    }
}