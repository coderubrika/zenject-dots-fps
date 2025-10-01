using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class AttackTargetAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        
        public class Baker : Baker<AttackTargetAuthoring>
        {
            public override void Bake(AttackTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AttackTarget
                {
                    Target = authoring.target != null 
                        ? GetEntity(authoring.target, TransformUsageFlags.Dynamic) 
                        : Entity.Null
                });
            }
        }
    }

    public struct AttackTarget : IComponentData
    {
        public Entity Target;
    }
}