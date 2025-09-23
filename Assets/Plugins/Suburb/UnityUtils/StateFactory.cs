using System;
using System.Collections.Generic;

namespace Suburb.Utils
{
    public abstract class StateFactory<T>
    {
        private readonly Dictionary<Type, T> states = new();

        public TState Get<TState>()
            where TState : T
        {
            Type typeState = typeof(TState);
            if (states.TryGetValue(typeState, out var state))
                return (TState)state;

            TState newState = Create<TState>();
            states.Add(typeState, newState);
            return newState;
        }

        protected abstract TState Create<TState>()
            where TState : T;
    }
}