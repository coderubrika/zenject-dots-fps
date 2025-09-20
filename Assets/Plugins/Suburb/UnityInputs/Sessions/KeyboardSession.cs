using System;
using Suburb.Utils;
using UnityEngine.InputSystem;

namespace Suburb.Inputs
{
    public class KeyboardSession : IDisposable
    {
        private readonly Func<KeyboardSession, Key, IObservable<bool>> eventsGetter;
        
        private Action onDispose;
        
        public KeyboardSession(Func<KeyboardSession, Key, IObservable<bool>> eventsGetter, Action onDispose)
        {
            this.onDispose = onDispose;
            this.eventsGetter = eventsGetter;
        }

        public IObservable<bool> OnKey(Key key)
        {
            return eventsGetter.Invoke(this, key);
        }

        public void Dispose()
        {
            onDispose?.Invoke();
            onDispose = null;
        }
    }
}