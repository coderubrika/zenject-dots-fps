using Suburb.Utils;

namespace TestRPG.GameStates
{
    public class GameStateFactory : StateFactory<IGameState>
    {
        private readonly InjectCreator injectCreator;
        
        public GameStateFactory(InjectCreator injectCreator)
        {
            this.injectCreator = injectCreator;
        }
        
        protected override TState Create<TState>()
        {
            return injectCreator.Create<TState>();
        }
    }
}