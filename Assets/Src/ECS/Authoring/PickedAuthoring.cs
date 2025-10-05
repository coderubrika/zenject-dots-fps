using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class PickedAuthoring : MonoBehaviour
    {
        [SerializeField] private bool isEnabled;
        public class Baker : Baker<PickedAuthoring>
        {
            public override void Bake(PickedAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Picked());
                SetComponentEnabled<Picked>(entity, authoring.isEnabled);
            }
        }
    }
    
    public struct Picked : IComponentData, IEnableableComponent
    {
    }
}