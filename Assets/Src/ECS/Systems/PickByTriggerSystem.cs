using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PickByTriggerSystem : ISystem
    {
        private ComponentLookup<Pick> picks;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TriggerEventData>();
            state.RequireForUpdate<Pick>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            picks = state.GetComponentLookup<Pick>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            picks.Update(ref state);
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new PickByTriggerJob
            {
                ECB = ecb.AsParallelWriter(),
                Picks = picks
            };

            state.Dependency = job.Schedule(state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct PickByTriggerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ComponentLookup<Pick> Picks;
        
        [BurstCompile]
        private void Execute([ChunkIndexInQuery] int chunkIndex, in TriggerEventData eventData, in Entity entity)
        {
            if (!Picks.HasComponent(eventData.OtherEntity))
                return;

            Pick pick = Picks[eventData.OtherEntity];
            ECB.DestroyEntity(chunkIndex, eventData.OtherEntity);
            ECB.RemoveComponent<TriggerEventData>(chunkIndex, entity);
            ECB.SetComponentEnabled<Picked>(chunkIndex, pick.PickEntity, true);
        }
    }
}