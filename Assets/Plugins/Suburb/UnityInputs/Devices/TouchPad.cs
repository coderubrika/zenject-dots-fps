using System;
using UniRx;
using UnityEngine;
using Suburb.Utils;

namespace Suburb.Inputs
{
    public class TouchPad : MonoBehaviour
    {
        [SerializeField] private RectTransform touchRect;
        [SerializeField] private float sensitivity;
        [SerializeField] private float smoothnessToHold;
        [SerializeField] private float smoothnessToDrag;
        
        public RectTransform TouchRect => touchRect;
        
        private IDisposable dragDisposable;
        private IDisposable updateDisposable;
        private bool isDataArrived;
        private Vector2 data;
        
        public ReactiveProperty<Vector2> Input { get; } = new();
        
        private readonly CompositeDisposable compositeDisposable = new();
        
        public IDisposable Connect(SwipeMember swipe)
        {
            swipe.OnDown
                .Subscribe(_ =>
                {
                    Input.Value = Vector2.zero;
                    dragDisposable?.Dispose();
                    updateDisposable?.Dispose();
                    
                    dragDisposable = swipe.OnDrag
                        .Subscribe(Drag)
                        .AddTo(compositeDisposable);
                    
                    updateDisposable = Observable.EveryUpdate()
                        .Subscribe(_ => SendDrag())
                        .AddTo(compositeDisposable);
                })
                .AddTo(compositeDisposable);
            
            swipe.OnUp
                .Subscribe(_ =>
                {
                    Input.Value = Vector2.zero;
                    dragDisposable?.Dispose();
                    updateDisposable?.Dispose();
                })
                .AddTo(compositeDisposable);
            
            return Disposable.Create(compositeDisposable.Clear);
        }

        private void Drag(Vector2 delta)
        {
            Vector2 rectDrag = touchRect.InverseTransformVector(delta);
            isDataArrived = true;
            data = (rectDrag / sensitivity).Clamp(-1,1);
        }

        private void SendDrag()
        {
            if (isDataArrived)
            {
                Input.Value = Vector2.Lerp(Input.Value, data, Time.deltaTime * smoothnessToDrag);
                isDataArrived = false;
                return;
            }
                            
            Input.Value = Vector2.Lerp(Input.Value, Vector2.zero, Time.deltaTime * smoothnessToHold);
        }
    }
}