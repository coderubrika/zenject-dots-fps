using Unity.Burst;
using Unity.Entities;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct DeathByHealthSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Health>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new DeathByHealthSystemJob
            {
                ECB = ecb.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    [WithNone(typeof(DeathTag))]
    public partial struct DeathByHealthSystemJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(
            [EntityIndexInQuery] int entityIndex,
            Entity entity,
            in Health health)
        {
            if (health.Value <= 0)
                ECB.AddComponent<DeathTag>(entityIndex, entity);
        }
    }
}