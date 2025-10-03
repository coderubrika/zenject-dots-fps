using System;
using Suburb.Utils;
using TestRPG.ECS;
using TestRPG.GameStates;
using UniRx;

namespace TestRPG.PlayerDir
{
    public class PlayerService
    {
        private readonly PlayerObject playerObject;
        private readonly StartGameSettingsRepository startGameSettingsRepository;
        private readonly StateRouter<IGameState> router;
        private readonly CompositeDisposable disposables = new();
        
        private IGameState state;
        private GameContext context;

        public PlayerData PlayerData => context.PlayerData;
        public IObservable<GameStateVariant> OnGameState => context.OnGameState;
        
        public PlayerService(
            PlayerObject playerObject,
            StartGameSettingsRepository startGameSettingsRepository,
            StateFactory<IGameState> stateFactory)
        {
            this.playerObject = playerObject;
            this.startGameSettingsRepository = startGameSettingsRepository;
            
            router = new StateRouter<IGameState>(ChangeState, stateFactory);
        }

        private void ChangeState(IGameState newState)
        {
            state = newState;
            state.Apply(router, context);
        }
        
        public void SetupPlayer()
        {
            context = new GameContext();
            router.GoTo<InitGameState>();
        }

        public void Clear()
        {
            disposables.Dispose();
            context.Dispose();
            playerObject.PlayerTransform.gameObject.SetActive(false);
        }
    }
}