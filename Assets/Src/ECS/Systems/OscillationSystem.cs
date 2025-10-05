using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    public partial struct OscillationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Rotation>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new OscillationJob
            {
                Time = (float)SystemAPI.Time.ElapsedTime
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
        
        [BurstCompile]
        public partial struct OscillationJob : IJobEntity
        {
            [ReadOnly] public float Time;

            [BurstCompile]
            private void Execute(in Oscillation oscillation, ref LocalTransform localTransform)
            {
                float3 deltaPosition = oscillation.Axis 
                                       * oscillation.Amplitude 
                                       * math.sin(math.radians(oscillation.Speed * Time));
                localTransform.Position = oscillation.LocalPosition + deltaPosition;
            }
        }
    }
}