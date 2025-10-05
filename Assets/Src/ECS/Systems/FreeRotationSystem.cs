using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FreeRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Rotation>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new FreeRotationJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
        
        [BurstCompile]
        public partial struct FreeRotationJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            [BurstCompile]
            private void Execute(in Rotation rotation, ref LocalTransform localTransform)
            {
                float3 worldOffset = localTransform.Position + math.rotate(localTransform.Rotation, rotation.Offset);
                float3 offsetPos = localTransform.Position - worldOffset;
    
                quaternion deltaRotation = quaternion.AxisAngle(
                    rotation.Axis, 
                    math.radians(rotation.Speed * DeltaTime));
    
                offsetPos = math.rotate(deltaRotation, offsetPos);
                localTransform.Position = offsetPos + worldOffset;
                localTransform.Rotation = math.mul(localTransform.Rotation, deltaRotation);
            }
        }
    }
}