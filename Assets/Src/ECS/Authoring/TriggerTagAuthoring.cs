using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class TriggerTagAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TriggerTagAuthoring>
        {
            public override void Bake(TriggerTagAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TriggerTag());
            }
        }
    }
    
    public struct TriggerTag : IComponentData {}

    public struct TriggerEventData : IComponentData
    {
        public Entity OtherEntity;
    }
}