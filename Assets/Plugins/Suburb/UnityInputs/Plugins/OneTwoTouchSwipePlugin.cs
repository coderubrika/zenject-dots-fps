using System.Linq;
using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class OneTwoTouchSwipePlugin : IInputPlugin
    {
        private readonly TouchProvider touchProvider;
        private readonly CompositeDisposable touchProviderDisposables = new();
        private readonly CompositeDisposable compositorDisposables = new();
        
        private OneTwoTouchPluginCompositor compositor;
        private SwipeMember member;
        private GestureType gestureType;
        private bool isBothTouches;
        
        public bool SetSender(object sender)
        {
            compositor = sender as OneTwoTouchPluginCompositor;

            if (compositor == null)
                return false;
            
            compositor.First
                .Skip(1)
                .Where(_ => !isBothTouches)
                .Subscribe(SetFirstTouchId)
                .AddTo(compositorDisposables);

            compositor.OnBothBegin
                .Subscribe(_ => isBothTouches = true)
                .AddTo(compositorDisposables);

            compositor.OnBothEnd
                .Subscribe(_ =>
                {
                    SetBothTouchIds(compositor.First.Value, compositor.Second.Value);
                    isBothTouches = false;
                })
                .AddTo(compositorDisposables);
            
            return true;
        }

        public void Unlink()
        {
            member = null;
            compositor = null;
            touchProviderDisposables.Clear();
            compositorDisposables.Clear();
        }

        public OneTwoTouchSwipePlugin(TouchProvider touchProvider)
        {
            this.touchProvider = touchProvider;
            gestureType = GestureType.None;
        }
        
        private void SetFirstTouchId(int id)
        {
            if (id == -1)
            {
                HandleUp(touchProvider.UpEvents
                    .First(item => item.Id == compositor.PreviousFirst)
                    .Position);
                return;
            }

            if (id == compositor.PreviousSecond)
                return;
            
            gestureType = GestureType.Down;
            var pointer = touchProvider.DownEvents.First(item => item.Id == id);
            member.PutDown(pointer.Position);
            
            touchProvider.OnDragStart
                .Where(_ => gestureType == GestureType.Down)
                .Subscribe(_ => DragStart())
                .AddTo(touchProviderDisposables);
                
            touchProvider.OnDrag
                .Where(_ => gestureType == GestureType.Drag)
                .Subscribe(_ => Drag())
                .AddTo(touchProviderDisposables);
        }

        private void SetBothTouchIds(int firstId, int secondId)
        {
            if (firstId == -1)
            {
                var upPointers = touchProvider.UpEvents
                    .Where(item => item.Id == compositor.PreviousFirst || item.Id == compositor.PreviousSecond)
                    .ToArray();
                HandleUp((upPointers[0].Position + upPointers[1].Position) * 0.5f);
                return;
            }
            
            gestureType = GestureType.Down;
            var pointers = touchProvider.DownEvents
                .Where(item => item.Id == firstId || item.Id == secondId)
                .ToArray();
            
            member.PutDown((pointers[0].Position + pointers[1].Position) * 0.5f);
            
            touchProvider.OnDragStart
                .Where(_ => gestureType == GestureType.Down)
                .Subscribe(_ => DragStart())
                .AddTo(touchProviderDisposables);
                
            touchProvider.OnDrag
                .Where(_ => gestureType == GestureType.Drag)
                .Subscribe(_ => Drag())
                .AddTo(touchProviderDisposables);
        }
        
        public bool SetReceiver(object swipeCandidate)
        {
            member = swipeCandidate as SwipeMember;
            return member != null;
        }

        private void DragStart()
        {
            var pointers = touchProvider.DragStartEvents
                .Where(item => item.Id == compositor.First.Value || item.Id == compositor.Second.Value)
                .ToArray();
            
            if (pointers.Length == 0)
                return;
            
            if (pointers.Length == 1)
            {
                if (compositor.TouchCount == 1)
                    member.PutDragStart(pointers[0].Delta);
                else
                    member.PutDragStart(pointers[0].Delta * 0.5f);
            }
            
            if (pointers.Length == 2)
                member.PutDragStart((pointers[0].Delta + pointers[1].Delta) * 0.5f);
            
            gestureType = GestureType.Drag;
        }

        private void Drag()
        {
            var pointers = touchProvider.DragEvents
                .Where(item => item.Id == compositor.First.Value || item.Id == compositor.Second.Value)
                .ToArray();
            
            if (pointers.Length == 0)
                return;

            gestureType = GestureType.Drag;
            
            if (pointers.Length == 1)
            {
                if (compositor.TouchCount == 1)
                    member.PutDrag(pointers[0].Delta);
                else
                    member.PutDrag(pointers[0].Delta * 0.5f);
            }
            
            if (pointers.Length == 2)
                member.PutDrag((pointers[0].Delta + pointers[1].Delta) * 0.5f);
        }

        private void HandleUp(Vector2 position)
        {
            if (gestureType == GestureType.Drag) 
                member.PutDragEnd();
                
            gestureType = GestureType.None;
            touchProviderDisposables.Clear();
            member.PutUp(position);
        }
    }
}