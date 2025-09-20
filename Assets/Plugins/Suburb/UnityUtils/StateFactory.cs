using System;
using System.Collections.Generic;

namespace Suburb.Utils
{
    public class StateFactory<T>
    {
        private readonly Dictionary<Type, T> states = new();

        public TState Get<TState>()
            where TState : T, new()
        {
            Type typeState = typeof(TState);
            if (states.TryGetValue(typeState, out var state))
                return (TState)state;

            TState newState = new TState();
            states.Add(typeState, newState);
            return newState;
        }
    }
}