using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UniRx;

namespace Suburb.Inputs
{
    public class KeyboardInputProvider
    {
        private readonly Dictionary<Key, KeyData> keySessions = new();
        private readonly HashSet<KeyboardSession> sessions = new();
        private readonly List<Key> keysForRemove = new();
        
        private Dictionary<Key, KeyData>.KeyCollection currentKeys;
        private IDisposable updateDisposable;

        public KeyboardSession CreateSession()
        {
            if (sessions.Count == 0)
                Enable();
            KeyboardSession session = null;
            session = new KeyboardSession(SubscribeSessionOnKey, () =>
            {
                foreach (var key in keySessions.Keys)
                {
                    var keyData = keySessions[key];
                    keyData.Sessions.Remove(session);
                    if (keyData.Sessions.Count == 0)
                    {
                        keyData.OnPressed.OnCompleted();
                        keyData.OnPressed.Dispose();
                        keysForRemove.Add(key);
                    }
                }

                foreach (var key in keysForRemove)
                    keySessions.Remove(key);
                
                sessions.Remove(session);
                currentKeys = keySessions.Keys;
                if (sessions.Count == 0)
                    Disable();
            });
            sessions.Add(session);
            return session;
        }

        private IObservable<bool> SubscribeSessionOnKey(KeyboardSession session, Key key)
        {
            if (!sessions.Contains(session))
                return null;

            if (keySessions.TryGetValue(key, out KeyData keyData))
            {
                keyData.Sessions.Add(session);
                return keyData.OnPressed;
            }

            KeyData newKeyData = new KeyData();
            newKeyData.Sessions.Add(session);
            keySessions.Add(key, newKeyData);
            currentKeys = keySessions.Keys;
            return newKeyData.OnPressed;
        }
        
        private void Enable()
        {
            updateDisposable = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (currentKeys == null)
                        return;
                    
                    foreach (var key in currentKeys)
                    {
                        KeyData keyData = keySessions[key];
                        bool isPressed = Keyboard.current[key].isPressed;
                        if (keyData.IsPressed == isPressed)
                            continue;
                        keyData.IsPressed = isPressed;
                        keyData.OnPressed.OnNext(isPressed);
                    }
                });
        }

        private void Disable()
        {
            updateDisposable?.Dispose();
        }
        
        private class KeyData
        {
            public bool IsPressed;
            public readonly HashSet<KeyboardSession> Sessions = new();
            public readonly Subject<bool> OnPressed = new();
        }
    }
}