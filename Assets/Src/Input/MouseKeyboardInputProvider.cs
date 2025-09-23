using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestRPG.Input
{
    public class MouseKeyboardInputProvider : IPlayerInputProvider
    {
        private readonly InjectCreator injectCreator;
        private readonly KeyboardCross keyboardCross;
        private readonly MovePad movePad;
        private readonly RectBasedSession movePadSession;
        private readonly RectBasedSession fireSession;
        private readonly LayerOrderer layerOrderer;
        
        private readonly CompositeDisposable disposables = new();

        public ReactiveProperty<(Vector2 Direction, float Force)> MoveDirectionAndForce =>
            keyboardCross.DirectionAndForce;
        
        public ReactiveProperty<Vector2> RotateAxes => movePad.Input;
        
        public ReactiveProperty<bool> IsFire { get; } = new();

        public MouseKeyboardInputProvider(
            InjectCreator injectCreator,
            InputLayoutService inputLayoutService,
            LayerOrderer layerOrderer)
        {
            this.layerOrderer = layerOrderer;
            movePad = inputLayoutService.InputLayout.MovePad;
            keyboardCross = injectCreator.Create<KeyboardCross>();

            movePadSession = new RectBasedSession(movePad.transform as RectTransform);
            fireSession = new RectBasedSession(movePad.transform as RectTransform);
            
            var mouseMoveCompositor = injectCreator.Create<MouseMoveCompositor>();
            movePadSession.AddCompositor(mouseMoveCompositor);
            var mouseSwipeCompositor = injectCreator.Create<MouseSwipeCompositor>(MouseButtonType.Left);
            fireSession.AddCompositor(mouseSwipeCompositor);
        }
        
        public void Enable()
        {
            EnableMovePad();
            EnableKeyboardCross();
            EnableFire();
        }

        public void Disable()
        {
            disposables.Clear();
            MoveDirectionAndForce.Value = (Vector2.zero, 0);
        }

        private void EnableMovePad()
        {
            movePad.Connect(movePadSession.GetMember<MoveMember>())
                .AddTo(disposables);
            
            layerOrderer.ConnectFirst(movePadSession)
                .AddTo(disposables);
        }

        private void EnableKeyboardCross()
        {
            keyboardCross.Connect(Key.A, Key.D, Key.W, Key.S)
                .AddTo(disposables);
        }

        private void EnableFire()
        {
            var swipeMember = fireSession.GetMember<SwipeMember>();
            
            swipeMember.OnDown
                .Subscribe(_ => IsFire.Value = true)
                .AddTo(disposables);

            swipeMember.OnUp
                .Subscribe(_ => IsFire.Value = false)
                .AddTo(disposables);
            
            layerOrderer.ConnectFirst(fireSession)
                .AddTo(disposables);
        }
    }
}