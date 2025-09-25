using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TestRPG.ECS
{
    public class PlayerCameraRotationAuthoring : MonoBehaviour
    {
        [SerializeField] private float horizontalSpeed;
        [SerializeField] private float verticalSpeed;
        [SerializeField] private float verticalAngleRange;
        
        public class Baker : Baker<PlayerCameraRotationAuthoring>
        {
            public override void Bake(PlayerCameraRotationAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerCameraRotation
                {
                    HorizontalSpeed = authoring.horizontalSpeed,
                    VerticalSpeed = authoring.verticalSpeed,
                    VerticalAngleRange = authoring.verticalAngleRange
                });
            }
        }
    }

    public struct PlayerCameraRotation : IComponentData
    {
        public float HorizontalSpeed;
        public float VerticalSpeed;
        public float VerticalAngleRange;
    }
}