using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PlayerMoveSystem : ISystem
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
                         physicsVelocity, 
                         transform, 
                         moveSpeed) 
                     in SystemAPI.Query<
                         RefRW<PhysicsVelocity>,
                         RefRO<LocalTransform>, 
                         RefRO<MoveSpeed>>().WithAll<Player>())
            {
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