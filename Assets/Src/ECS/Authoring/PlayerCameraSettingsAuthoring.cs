using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TestRPG.ECS
{
    public class PlayerCameraSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private float horizontalSpeed;
        [SerializeField] private float verticalSpeed;
        [SerializeField] private float verticalAngleRange;
        [SerializeField] private Vector3 cameraOffset;
        
        public class Baker : Baker<PlayerCameraSettingsAuthoring>
        {
            public override void Bake(PlayerCameraSettingsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerCameraSettings
                {
                    HorizontalSpeed = authoring.horizontalSpeed,
                    VerticalSpeed = authoring.verticalSpeed,
                    VerticalAngleRange = authoring.verticalAngleRange,
                    Offset = authoring.cameraOffset
                });
            }
        }
    }

    public struct PlayerCameraSettings : IComponentData
    {
        public float HorizontalSpeed;
        public float VerticalSpeed;
        public float VerticalAngleRange;
        public float3 Offset;
    }
}