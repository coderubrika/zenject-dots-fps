using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct StopGameSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingleton<GameState>();

            if (gameState.State == GameStateVariant.Stop)
            {
                var systemHandles = state.WorldUnmanaged.GetAllSystems(Allocator.Temp);

                foreach (var systemHandle in systemHandles)
                {
                    if (systemHandle == state.SystemHandle)
                        continue;
                    
                    SystemState systemState = state.WorldUnmanaged.ResolveSystemStateRef(systemHandle);
                    systemState.Enabled = false;
                }
                
                systemHandles.Dispose();
            }
        }
    }
}