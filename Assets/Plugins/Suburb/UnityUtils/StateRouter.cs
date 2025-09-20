using System;

namespace Suburb.Utils
{
    public class StateRouter<T>
    {
        private readonly Action<T> stateCallback;
        private readonly StateFactory<T> factory;
        
        public StateRouter(
            Action<T> stateCallback,
            StateFactory<T> factory)
        {
            this.factory = factory;
            this.stateCallback = stateCallback;
        }
        
        public void GoTo<TState>() where TState : T, new()
        {
            stateCallback.Invoke(factory.Get<TState>());
        }
    }
}