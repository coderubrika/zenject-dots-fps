using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class TwoTouchZoomPlugin : IInputPlugin
    {
        private readonly TouchProvider touchProvider;
        private readonly CompositeDisposable touchProviderDisposables = new();
        private readonly CompositeDisposable compositorDisposables = new();
        
        private OneTwoTouchPluginCompositor compositor;
        private ZoomMember member;

        private Vector2 firstPosition;
        private Vector2 secondPosition;
        private float previousDistance;
        private float distance;
        
        public TwoTouchZoomPlugin(TouchProvider touchProvider)
        {
            this.touchProvider = touchProvider;
        }
        
        public bool SetSender(object sender)
        {
            compositor = sender as OneTwoTouchPluginCompositor;

            if (compositor == null)
                return false;
            
            compositor.Second
                .Skip(1)
                .Subscribe(SetSecondTouchId)
                .AddTo(compositorDisposables);
            
            return true;
        }

        public bool SetReceiver(object zoomCandidate)
        {
            member = zoomCandidate as ZoomMember;
            return member != null;
        }
        
        
        public void Unlink()
        {
            member = null;
            compositor = null;
            touchProviderDisposables.Clear();
            compositorDisposables.Clear();
        }
        
        private void SetSecondTouchId(int id)
        {
            if (id == -1)
            {
                firstPosition = Vector2.zero;
                secondPosition = Vector2.zero;
                previousDistance = 0;
                distance = 0;
                touchProviderDisposables.Clear();
                return;
            }
            
            firstPosition = touchProvider.GetEventData(compositor.First.Value).Position;
            secondPosition = touchProvider.GetEventData(id).Position;
            previousDistance = Vector2.Distance(firstPosition, secondPosition);
            
            touchProvider.OnDragStart
                .Subscribe(_ => Zoom(touchProvider.DragStartEvents))
                .AddTo(touchProviderDisposables);
            
            touchProvider.OnDrag
                .Subscribe(_ => Zoom(touchProvider.DragEvents))
                .AddTo(touchProviderDisposables);
        }

        private void Zoom(IEnumerable<PointerEventData> events)
        {
            var pointers = events
                .Where(item => item.Id == compositor.First.Value || item.Id == compositor.Second.Value)
                .ToArray();
            
            if (pointers.Length == 0)
                return;

            if (pointers.Length == 1)
            {
                var pointer = pointers[0];
                if (pointer.Id == compositor.First.Value)
                    firstPosition = pointer.Position;
                else
                    secondPosition = pointer.Position;
            }
            else
            {
                firstPosition = pointers.First(item => item.Id == compositor.First.Value).Position;
                secondPosition = pointers.First(item => item.Id == compositor.Second.Value).Position;
            }
            
            distance = Vector2.Distance(firstPosition, secondPosition);
            member.PutZoom(distance / previousDistance, (firstPosition + secondPosition) * 0.5f);
            previousDistance = distance;
        }
    }
}