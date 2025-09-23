using UniRx;
using UnityEngine;

namespace TestRPG.Input
{
    public interface IPlayerInputProvider
    {
        public ReactiveProperty<(Vector2 Direction, float Force)> MoveDirectionAndForce { get; }
        public ReactiveProperty<Vector2> RotateAxes { get; }
        public ReactiveProperty<bool> IsFire { get; }

        public void Enable();
        public void Disable();
    }
}