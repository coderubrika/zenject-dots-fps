using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class CanAttackAuthoring : MonoBehaviour
    {
        public class Baker : Baker<CanAttackAuthoring>
        {
            public override void Bake(CanAttackAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CanAttack());
            }
        }
    }

    public struct CanAttack : IComponentData
    {
        public bool Value;
    }
}