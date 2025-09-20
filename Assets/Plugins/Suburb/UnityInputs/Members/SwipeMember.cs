using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class SwipeMember
    {
        public ReactiveCommand<Vector2> OnDown { get; } = new();
        public ReactiveCommand<Vector2> OnDragStart { get; } = new();
        public ReactiveCommand<Vector2> OnDrag { get; } = new();
        public ReactiveCommand OnDragEnd { get; } = new();
        public ReactiveCommand<Vector2> OnUp { get; } = new();
        
        public void PutDown(Vector2 position)
        {
            OnDown.Execute(position);
        }

        public void PutUp(Vector2 position)
        {
            OnUp.Execute(position);
        }

        public void PutDragStart(Vector2 delta)
        {
            OnDragStart.Execute(delta);
        }

        public void PutDrag(Vector2 delta)
        {
            OnDrag.Execute(delta);
        }

        public void PutDragEnd()
        {
            OnDragEnd.Execute();
        }
    }
}