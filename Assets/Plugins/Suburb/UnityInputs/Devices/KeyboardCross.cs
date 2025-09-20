using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Suburb.Inputs
{
    public class KeyboardCross : IDisposable
    {
        private readonly KeyboardSession keyboardSession;
        
        private Key left, right, up, down;
        private Vector2 moveDirectionFromKeyboard;

        private readonly CompositeDisposable disposables = new();
        
        public ReactiveProperty<(Vector2 Direction, float Force)> DirectionAndForce { get; } = new();


        public KeyboardCross(KeyboardInputProvider keyboardInputProvider)
        {
            keyboardSession = keyboardInputProvider.CreateSession();
        }
        
        public IDisposable Connect(Key left, Key right, Key up, Key down)
        {
            DirectionAndForce.Value = (Vector2.zero, 0);
            moveDirectionFromKeyboard = Vector2.zero;
            
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
            SetupKeys();
            
            return Disposable.Create(() =>
            {
                moveDirectionFromKeyboard = Vector2.zero;
                disposables.Clear();
            });
        }

        private void SetupKeys()
        {
            keyboardSession.OnKey(left)
                .Subscribe(isPressed =>
                {
                    if (!isPressed && moveDirectionFromKeyboard.x == 0)
                        return;
                    
                    moveDirectionFromKeyboard.x += isPressed ? -1 : 1;
                    DirectionAndForce.Value = (moveDirectionFromKeyboard.normalized, 1);
                })
                .AddTo(disposables);
            
            keyboardSession.OnKey(right)
                .Subscribe(isPressed =>
                {
                    if (!isPressed && moveDirectionFromKeyboard.x == 0)
                        return;
                    
                    moveDirectionFromKeyboard.x += isPressed ? 1 : -1;
                    DirectionAndForce.Value = (moveDirectionFromKeyboard.normalized, 1);
                })
                .AddTo(disposables);
            
            keyboardSession.OnKey(up)
                .Subscribe(isPressed =>
                {
                    if (!isPressed && moveDirectionFromKeyboard.y == 0)
                        return;
                    
                    moveDirectionFromKeyboard.y += isPressed ? 1 : -1;
                    DirectionAndForce.Value = (moveDirectionFromKeyboard.normalized, 1);
                })
                .AddTo(disposables);
            
            keyboardSession.OnKey(down)
                .Subscribe(isPressed =>
                {
                    if (!isPressed && moveDirectionFromKeyboard.y == 0)
                        return;
                    
                    moveDirectionFromKeyboard.y += isPressed ? -1 : 1;
                    DirectionAndForce.Value = (moveDirectionFromKeyboard.normalized, 1);
                })
                .AddTo(disposables);
        }

        public void Dispose()
        {
            keyboardSession.Dispose();
            disposables.Dispose();
            DirectionAndForce.Dispose();
        }
    }
}