using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TestRPG.ECS
{
    public class RotationAuthoring : MonoBehaviour
    {
        [SerializeField] private Vector3 axis;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float speed;
        
        
        public class Baker : Baker<RotationAuthoring>
        {
            public override void Bake(RotationAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Rotation
                {
                    Axis = authoring.axis,
                    Speed = authoring.speed,
                    Offset = authoring.offset
                });
            }
        }
    }

    public struct Rotation : IComponentData, IEnableableComponent
    {
        public float3 Axis;
        public float Speed;
        public float3 Offset;
    }
}