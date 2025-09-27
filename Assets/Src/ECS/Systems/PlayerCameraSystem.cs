using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerCameraSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInput>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PlayerInput playerInput = SystemAPI.GetSingleton<PlayerInput>();

            foreach (var (
                         transform,
                         playerCameraSettings,
                         cameraData,
                         player) 
                     in SystemAPI.Query<
                         RefRW<LocalTransform>, 
                         RefRO<PlayerCameraSettings>, 
                         RefRW<CameraData>,
                         RefRO<Player>>().WithAll<Player>())
            {
                var horizontalRotation = quaternion.RotateY(
                    math.radians(playerInput.RotateAxis.y * SystemAPI.Time.DeltaTime * playerCameraSettings.ValueRO.HorizontalSpeed));

                quaternion newPlayerRotation = math.mul(transform.ValueRO.Rotation, horizontalRotation);
                transform.ValueRW.Rotation = newPlayerRotation;
        
                Entity cameraEntity = player.ValueRO.CameraObject;
                var cameraTransform = SystemAPI.GetComponentRW<LocalTransform>(cameraEntity);
                
                cameraData.ValueRW.VerticalAngle -= playerInput.RotateAxis.x * SystemAPI.Time.DeltaTime * playerCameraSettings.ValueRO.VerticalSpeed;
                cameraData.ValueRW.VerticalAngle = math.clamp(cameraData.ValueRO.VerticalAngle, 
                    -playerCameraSettings.ValueRO.VerticalAngleRange, 
                    playerCameraSettings.ValueRO.VerticalAngleRange);
                
                quaternion verticalRotation = quaternion.RotateX(math.radians(cameraData.ValueRO.VerticalAngle));
                quaternion newCameraRotation = math.mul(newPlayerRotation, verticalRotation);

                cameraTransform.ValueRW.Rotation = newCameraRotation;
                cameraTransform.ValueRW.Position = transform.ValueRO.Position + playerCameraSettings.ValueRO.Offset;
            }
        }
    }
}