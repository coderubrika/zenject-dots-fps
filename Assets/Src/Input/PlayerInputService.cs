using UniRx;
using UnityEngine;

namespace TestRPG.Input
{
    public class PlayerInputService
    {
        public (Vector2 Direction, float Force) MoveDirectionAndForce { get; private set; }
        public Vector2 RotateAxes { get; private set; } = Vector2.zero;
        public bool Fire { get; private set; }
        
        private IPlayerInputProvider inputProvider;
        private bool isEnabled;

        private readonly CompositeDisposable disposables = new();
        
        public void SetInputProvider(IPlayerInputProvider inputProvider)
        {
            bool isNeedEnable = false;
            
            if (isEnabled)
            {
                Disable();
                isNeedEnable = true;
            }
            
            this.inputProvider = inputProvider;
            
            if (isNeedEnable)
                Enable();
        }
        
        public void Enable()
        {
            if (isEnabled)
                return;

            isEnabled = true;
            
            inputProvider.IsFire
                .Subscribe(isOn => Fire = isOn)
                .AddTo(disposables);

            inputProvider.MoveDirectionAndForce
                .Subscribe(eventData => MoveDirectionAndForce = eventData)
                .AddTo(disposables);

            inputProvider.RotateAxes
                .Subscribe(delta => RotateAxes = delta)
                .AddTo(disposables);
            
            // Observable.EveryLateUpdate()
            //     .Subscribe(_ => ResetVectors())
            //     .AddTo(disposables);
            
            inputProvider.Enable();
        }

        public void Disable()
        {
            if (!isEnabled)
                return;

            isEnabled = false;
            MoveDirectionAndForce = (Vector2.zero, 0);
            RotateAxes = Vector2.zero;
            Fire = false;
            disposables.Clear();
            inputProvider.Disable();
        }
    }
}