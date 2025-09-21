using Suburb.Utils;
using Zenject;

namespace TestRPG 
{
    public class Startup : IInitializable
    {
        private readonly ScreenService screenService;

        public Startup(ScreenService screenService)
        {
            this.screenService = screenService;
        }
        
        public void Initialize()
        {
            this.Log("Startup initialize");
            screenService.GoTo<StartScreen>();
        }
    }
}
