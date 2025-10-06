using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct TriggerSystem : ISystem
    {
        private ComponentLookup<TriggerTag> triggerTags;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<TriggerTag>();
            triggerTags = state.GetComponentLookup<TriggerTag>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            triggerTags.Update(ref state);
            var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new TriggerJob
            {
                TriggerTags = triggerTags,
                ECB = ecb.AsParallelWriter()
            };
            
            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = job.Schedule(simulationSingleton, state.Dependency);
        }
    }
    
    [BurstCompile]
    public struct TriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<TriggerTag> TriggerTags;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (TriggerTags.HasComponent(entityA))
                AddEventData(entityB, entityA);
            else if (TriggerTags.HasComponent(entityB))
                AddEventData(entityA, entityB);
        }

        [BurstCompile]
        private void AddEventData(in Entity main, in Entity other)
        {
            ECB.AddComponent(main.Index, main, new TriggerEventData
            {
                OtherEntity = other
            });
        }
    }
}