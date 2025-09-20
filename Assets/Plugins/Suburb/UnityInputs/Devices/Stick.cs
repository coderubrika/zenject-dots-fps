using System;
using DG.Tweening;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class Stick : MonoBehaviour
    {
        [SerializeField] private RectTransform joystickOrigin;
        [SerializeField] private RectTransform joystickHandler;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private readonly CompositeDisposable compositeDisposable = new();
        
        private float rectRadius;
        private Vector2 currentPosition;
        private Vector2 currentDelta;
        
        public ReactiveProperty<(Vector2 Direction, float Force)> DirectionAndForce { get; } = new();
        
        public IDisposable Connect(SwipeMember swipe)
        {
            DirectionAndForce.Value = (Vector2.zero, 0);
            canvasGroup.alpha = 0;
            rectRadius = joystickOrigin.sizeDelta.x / 2;
            swipe.OnDown
                .Subscribe(position =>
                {
                    joystickOrigin.position = position;
                    joystickHandler.position = position;
                    
                    currentPosition = position;
                    currentDelta = Vector2.zero;
                    DirectionAndForce.Value = (Vector2.zero, 0);
                    DOTween.Kill(canvasGroup);
                    canvasGroup.DOFade(1, 0.4f);
                })
                .AddTo(compositeDisposable);

            swipe.OnDrag
                .Subscribe(newDelta =>
                {
                    currentPosition += newDelta;
                    currentDelta += newDelta;

                    var anchoredPosition = joystickOrigin.InverseTransformPoint(currentPosition).To2();
                    float magnitude = anchoredPosition.magnitude;
                    Vector2 normAncPos = anchoredPosition / magnitude;
                    magnitude = magnitude > rectRadius ? rectRadius : magnitude;
                    anchoredPosition = normAncPos * magnitude;
                    joystickHandler.anchoredPosition = anchoredPosition;
                    DirectionAndForce.Value = (normAncPos, magnitude / rectRadius);
                })
                .AddTo(compositeDisposable);
            
            swipe.OnUp
                .Subscribe(_ =>
                {
                    DirectionAndForce.Value = (Vector2.zero, 0);
                    DOTween.Kill(canvasGroup);
                    canvasGroup.DOFade(0, 0.4f);
                })
                .AddTo(compositeDisposable);

            return Disposable.Create(Disconnect);
        }

        private void Disconnect()
        {
            compositeDisposable.Clear();
        }
    }
}