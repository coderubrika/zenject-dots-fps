using Unity.Burst;
using Unity.Entities;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(PickedDisableFXGroup))]
    public partial struct DisableRotationByPickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Picked>();
            state.RequireForUpdate<DisableRotationByPickTag>();
            state.RequireForUpdate<Rotation>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new DisableRotationByPickJob
            {
                ECB = ecb.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(Picked))]
    [WithAll(typeof(DisableRotationByPickTag))]
    [WithAll(typeof(Rotation))]
    public partial struct DisableRotationByPickJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity)
        {
            ECB.SetComponentEnabled<Rotation>(chunkIndex, entity, false);
        }
    }
    
    //-----------------------------------------------
    
    [UpdateInGroup(typeof(PickedDisableFXGroup))]
    public partial struct DisableOscillationByPickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Picked>();
            state.RequireForUpdate<DisableOscillationByPickTag>();
            state.RequireForUpdate<Oscillation>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new DisableOscillationByPickJob
            {
                ECB = ecb.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(Picked))]
    [WithAll(typeof(DisableOscillationByPickTag))]
    [WithAll(typeof(Oscillation))]
    public partial struct DisableOscillationByPickJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity)
        {
            ECB.SetComponentEnabled<Oscillation>(chunkIndex, entity, false);
        }
    }
}