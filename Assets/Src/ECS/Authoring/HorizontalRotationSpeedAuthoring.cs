using UnityEngine;
using Unity.Entities;

namespace TestRPG.ECS
{
    public class HorizontalRotationSpeedAuthoring : MonoBehaviour
    {
        [SerializeField] private float value;

        public class Baker : Baker<HorizontalRotationSpeedAuthoring>
        {
            public override void Bake(HorizontalRotationSpeedAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new HorizontalRotationSpeed
                {
                    Value = authoring.value
                });
            }
        }
    }

    public struct HorizontalRotationSpeed : IComponentData
    {
        public float Value;
    }
}