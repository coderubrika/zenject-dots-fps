using Suburb.Utils;
using TestRPG.Input;
using Zenject;

namespace TestRPG 
{
    public class Startup : IInitializable
    {
        private readonly ScreenService screenService;
        private readonly PlayerInputService playerInputService;
        private readonly IPlayerInputProvider playerInputProvider;
        
        public Startup(
            ScreenService screenService,
            PlayerInputService playerInputService,
            IPlayerInputProvider playerInputProvider)
        {
            this.playerInputService = playerInputService;
            this.screenService = screenService;
            this.playerInputProvider = playerInputProvider;
        }
        
        public void Initialize()
        {
            playerInputService.SetInputProvider(playerInputProvider);
            screenService.GoTo<StartScreen>();
        }
    }
}
