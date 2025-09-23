using Suburb.Utils;

namespace TestRPG.GameStates
{
    public interface IGameState
    {
        public void Apply(StateRouter<IGameState> router, GameContext gameContext);
    }
}