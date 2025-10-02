using Suburb.Utils;
using TestRPG.GameStates;

namespace TestRPG.PlayerDir
{
    public class PlayerService
    {
        private readonly PlayerObject playerObject;
        private readonly StartGameSettingsRepository startGameSettingsRepository;
        private readonly StateRouter<IGameState> router;
        
        private IGameState state;
        private GameContext context;

        public PlayerData PlayerData => context.PlayerData;
        
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
            context.Dispose();
            playerObject.PlayerTransform.gameObject.SetActive(false);
        }
    }
}