using Unity.Burst;
using Unity.Entities;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(StopGameSystem))]
    public partial struct PlayerDeathSystem : ISystem, ISystemStartStop
    {
        private bool isInit;
        private Entity playerEntity;
        private Entity gameEntity;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameState>();
            state.RequireForUpdate<Player>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!isInit)
            {
                isInit = true;
                playerEntity = SystemAPI.GetSingletonEntity<Player>();
                gameEntity = SystemAPI.GetSingletonEntity<GameState>();
            }

            var gameStateRO = SystemAPI.GetComponentRO<GameState>(gameEntity);

            var gameStateValue = gameStateRO.ValueRO.State;
            
            if (gameStateValue == GameStateVariant.Stop)
                return;
            
            if (!SystemAPI.HasComponent<DeathTag>(playerEntity))
                return;
            
            var gameStateRW = SystemAPI.GetComponentRW<GameState>(gameEntity);
            gameStateRW.ValueRW.State = GameStateVariant.Stop;
        }

        public void OnStartRunning(ref SystemState state) { }

        public void OnStopRunning(ref SystemState state)
        {
            isInit = false;
        }
    }
}