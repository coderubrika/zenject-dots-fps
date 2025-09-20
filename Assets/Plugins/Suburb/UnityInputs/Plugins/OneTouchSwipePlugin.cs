using System;
using System.Linq;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class OneTouchSwipePlugin : BaseInputPlugin<SwipeMember, OneTouchPluginCompositor>
    {
        private readonly TouchProvider touchProvider;

        private readonly CompositeDisposable touchProviderDisposables = new();
        
        private GestureType gestureType;
        private IDisposable idDisposable;
        
        public OneTouchSwipePlugin(TouchProvider touchProvider)
        {
            this.touchProvider = touchProvider;
            gestureType = GestureType.None;
        }

        public override bool SetSender(object sender)
        {
            if (!base.SetSender(sender))
                return false;

            idDisposable?.Dispose();
            idDisposable = Compositor.Id
                .Skip(1)
                .Subscribe(SetId);
            
            return true;
        }

        public override void Unlink()
        {
            base.Unlink();
            idDisposable?.Dispose();
        }

        private void SetId(int id)
        {
            if (id == -1)
            {
                HandleUp(touchProvider
                    .GetEventData(Compositor.PreviousId)
                    .Position);
                return;
            }
            
            gestureType = GestureType.Down;
            var pointer = touchProvider.DownEvents.First(item => item.Id == id);
            Member.PutDown(pointer.Position);
            
            touchProvider.OnDragStart
                .Where(_ => gestureType == GestureType.Down)
                .Subscribe(_ => DragStart())
                .AddTo(touchProviderDisposables);
                
            touchProvider.OnDrag
                .Where(_ => gestureType == GestureType.Drag)
                .Subscribe(_ => Drag())
                .AddTo(touchProviderDisposables);
        }

        private void DragStart()
        {
            var pointer = touchProvider.DragStartEvents
                .FirstOrDefault(item => item.Id == Compositor.Id.Value);
            
            if (pointer == null)
                return;
            
            Member.PutDragStart(pointer.Delta);
            gestureType = GestureType.Drag;
        }

        private void Drag()
        {
            var pointer = touchProvider.DragEvents
                .FirstOrDefault(item => item.Id == Compositor.Id.Value);
            
            if (pointer == null)
                return;
            
            Member.PutDrag(pointer.Delta);
        }
        
        private void HandleUp(Vector2 position)
        {
            if (gestureType == GestureType.Drag) 
                Member.PutDragEnd();
                
            gestureType = GestureType.None;
            touchProviderDisposables.Clear();
            Member.PutUp(position);
        }
    }
}