using UniRx;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using System;

namespace Suburb.Inputs
{
    public class MouseProvider
    {
        private readonly MouseControls inputControls;

        private IDisposable updateDisposable;
        private bool isEnabled;
        private int usersCount;
        
        public ReactiveCommand<MouseButtonType> OnDown { get; } = new();
        public ReactiveCommand<MouseButtonType> OnUp { get; } = new();
        public ReactiveCommand OnMove { get; } = new();
        public ReactiveCommand OnZoom { get; } = new();

        public Vector2 Position { get; private set; }
        public Vector2 Delta { get; private set; }
        public float Zoom { get; private set; }
        
        
        public MouseProvider()
        {
            inputControls = new MouseControls();

            inputControls.Mouse.DownLeft.performed += _ => Down(MouseButtonType.Left);
            inputControls.Mouse.DownRight.performed += _ => Down(MouseButtonType.Right);
            inputControls.Mouse.DownMiddle.performed += _ => Down(MouseButtonType.Middle);
            inputControls.Mouse.UpLeft.performed += _ => Up(MouseButtonType.Left);
            inputControls.Mouse.UpRight.performed += _ => Up(MouseButtonType.Right);
            inputControls.Mouse.UpMiddle.performed += _ => Up(MouseButtonType.Middle);
            inputControls.Mouse.Zoom.performed += SetZoom;
        }

        private void Disable()
        {
            if (usersCount == 0)
                return;

            usersCount -= 1;

            if (usersCount > 0)
                return;
            
            Position = Vector2.zero;
            Delta = Vector2.zero;
            updateDisposable?.Dispose();
            inputControls.Disable();
            isEnabled = false;
        }

        public IDisposable Enable()
        {
            usersCount += 1;
            if (isEnabled)
                return Disposable.Create(Disable);

            isEnabled = true;
            inputControls.Enable();

            updateDisposable = Observable.EveryUpdate()
                .Subscribe(_ => CalcPositionAndDelta());
            
            return Disposable.Create(Disable);
        }

        private void Down(MouseButtonType buttonType)
        {
            CalcPositionAndDelta();
            OnDown.Execute(buttonType);
        }

        private void Up(MouseButtonType buttonType)
        {
            CalcPositionAndDelta();
            OnUp.Execute(buttonType);
        }

        private void SetZoom(CallbackContext context)
        {
            Zoom = GetZoom(inputControls.Mouse.Zoom.ReadValue<Vector2>().y);
            OnZoom.Execute();
        }

        private float GetZoom(float wheel)
        {
            return (360f + wheel) / 360f;
        }
        
        private void CalcPositionAndDelta()
        {
            Delta = inputControls.Mouse.Delta.ReadValue<Vector2>();
            Position = inputControls.Mouse.Position.ReadValue<Vector2>();;
            
            if (Delta != Vector2.zero)
                OnMove.Execute();
        }
    }
}