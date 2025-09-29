using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class CloseDistanceAuthoring : MonoBehaviour
    {
        public class Baker : Baker<CloseDistanceAuthoring>
        {
            public override void Bake(CloseDistanceAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new IsCloseDistance());
            }
        }
    }

    public struct IsCloseDistance : IComponentData
    {
        public bool Value;
    }
}