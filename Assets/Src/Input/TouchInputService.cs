using UniRx;
using UnityEngine;

namespace TestRPG.Input
{
    public class TouchInputService : IPlayerInputProvider
    {
        public ReactiveProperty<(Vector2 Direction, float Force)> MoveDirectionAndForce { get; } = new();
        public ReactiveProperty<Vector2> RotateAxes { get; } = new();
        public ReactiveProperty<bool> IsFire { get; } = new();
        
        public void Enable()
        {
            
        }

        public void Disable()
        {
            
        }
    }
}