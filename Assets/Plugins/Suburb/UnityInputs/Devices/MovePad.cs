using System;
using UniRx;
using UnityEngine;
using Suburb.Utils;

namespace Suburb.Inputs
{
    public class MovePad : MonoBehaviour
    {
        [SerializeField] private RectTransform moveRect;
        [SerializeField] private float sensitivity;
        [SerializeField] private float smoothnessToHold;
        [SerializeField] private float smoothnessToDrag;
        
        private bool isDataArrived;
        private Vector2 data;
        
        public RectTransform MoveRect => moveRect;
        
        public ReactiveProperty<Vector2> Input { get; } = new();
        
        private readonly CompositeDisposable compositeDisposable = new();
        
        public IDisposable Connect(MoveMember move)
        {
            move.OnMove
                .Skip(1)
                .Subscribe(Move)
                .AddTo(compositeDisposable);
            
            Observable.EveryUpdate()
                .Subscribe(_ => SendMove())
                .AddTo(compositeDisposable);
            
            return Disposable.Create(compositeDisposable.Clear);
        }

        private void Move(Vector2 delta)
        {
            Vector2 rectDrag = moveRect.InverseTransformVector(delta);
            isDataArrived = true;
            data = (rectDrag / sensitivity).Clamp(-1,1);
        }

        private void SendMove()
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