using TestRPG;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    partial struct UnitMover : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (
                         localTransform, 
                         moveSpeed,
                         physicsVelocity) 
                     in SystemAPI.Query<
                         RefRW<LocalTransform>, 
                         RefRO<MoveSpeed>, 
                         RefRW<PhysicsVelocity>>())
            {
                float3 position = localTransform.ValueRO.Position;
                float3 targetPosition = position + new float3(5, 0, 10);
                float3 moveDirection = targetPosition - position;
                moveDirection = math.normalize(moveDirection);
            
                localTransform.ValueRW.Rotation = quaternion.LookRotation(moveDirection, math.up());
                physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.Value;
                physicsVelocity.ValueRW.Angular = float3.zero;
            }
        }
    }
}


