using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TestRPG.ECS
{
    public class FollowToTargetAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private float closeDistance;

        public class Baker : Baker<FollowToTargetAuthoring>
        {
            public override void Bake(FollowToTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FollowToTarget
                {
                    Target = authoring.target != null 
                        ? GetEntity(authoring.target, TransformUsageFlags.Dynamic) 
                        : Entity.Null,
                    
                    CloseDistance = authoring.closeDistance
                });
                AddComponent(entity, new FollowTarget());
            }
        }
    }

    public struct FollowToTarget : IComponentData
    {
        public Entity Target;
        public float CloseDistance;
    }

    public struct FollowTarget : IComponentData
    {
        public float3 Position;
    }
}