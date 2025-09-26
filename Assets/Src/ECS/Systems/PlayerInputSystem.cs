using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PlayerInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInput>();
        }

        public void OnUpdate(ref SystemState state)
        {
            PlayerInput playerInput = SystemAPI.GetSingleton<PlayerInput>();

            foreach (var (
                         physicsVelocity, 
                         transform, 
                         moveSpeed,
                         playerCameraRotation,
                         player) 
                     in SystemAPI.Query<
                         RefRW<PhysicsVelocity>,
                         RefRW<LocalTransform>, 
                         RefRO<MoveSpeed>,
                         RefRO<PlayerCameraRotation>, 
                         RefRO<Player>>())
            {
                var horizontalRotation = quaternion.RotateY(
                    playerInput.RotateAxis.y * SystemAPI.Time.DeltaTime * playerCameraRotation.ValueRO.HorizontalSpeed);
                
                transform.ValueRW.Rotation = math.mul(transform.ValueRW.Rotation, horizontalRotation);
                
                var cameraTransform = SystemAPI.GetComponent<LocalTransform>(player.ValueRO.CameraObject);
                
                var verticalRotation = quaternion.RotateX(
                    -playerInput.RotateAxis.x * SystemAPI.Time.DeltaTime * playerCameraRotation.ValueRO.VerticalSpeed);
                cameraTransform.Rotation = math.mul(cameraTransform.Rotation, verticalRotation);
                
                var angles = math.degrees(math.Euler(cameraTransform.Rotation));
                angles.x = math.clamp(angles.x, 
                    -playerCameraRotation.ValueRO.VerticalAngleRange, 
                    playerCameraRotation.ValueRO.VerticalAngleRange);
    
                cameraTransform.Rotation = quaternion.Euler(math.radians(angles));
    
                SystemAPI.SetComponent(player.ValueRO.CameraObject, cameraTransform);
                
                var moveDirection = new float3(playerInput.MoveDirectionAndForce.Direction.x, 0, 
                    playerInput.MoveDirectionAndForce.Direction.y);
                
                moveDirection = math.rotate(transform.ValueRO.Rotation, moveDirection);
                physicsVelocity.ValueRW.Linear = 
                    moveDirection * moveSpeed.ValueRO.Value * playerInput.MoveDirectionAndForce.Force;
                physicsVelocity.ValueRW.Angular = float3.zero;
            }
        }
    }
}