using Suburb.Utils;
using TestRPG.Input;

namespace TestRPG.GameStates
{
    public class StopGameState : IGameState
    {
        private readonly PlayerInputService playerInputService;
        
        public StopGameState( 
            PlayerInputService playerInputService)
        {
            this.playerInputService = playerInputService;
        }
        
        public void Apply(StateRouter<IGameState> router, GameContext gameContext)
        {
            playerInputService.Disable();
            gameContext.SendStopGame();
        }
    }
}