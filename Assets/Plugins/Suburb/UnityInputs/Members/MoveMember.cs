using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class MoveMember
    {
        public ReactiveCommand<Vector2> OnMove { get; } = new();
        
        public void PutMove(Vector2 delta)
        {
            OnMove.Execute(delta);
        }
    }
}