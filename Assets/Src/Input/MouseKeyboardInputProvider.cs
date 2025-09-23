using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TestRPG.Input
{
    public class MouseKeyboardInputProvider : IPlayerInputProvider, IInitializable
    {
        private readonly InjectCreator injectCreator;
        private readonly KeyboardCross keyboardCross;
        private readonly LayerOrderer layerOrderer;
        private readonly InputLayoutService inputLayoutService;
        
        private MovePad movePad;
        private RectBasedSession movePadSession;
        private RectBasedSession fireSession;
        
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
            this.injectCreator = injectCreator;
            this.inputLayoutService = inputLayoutService;
            this.layerOrderer = layerOrderer;
            
            keyboardCross = injectCreator.Create<KeyboardCross>();
        }
        
        public void Enable()
        {
            EnableMovePad();
            EnableKeyboardCross();
            EnableFire();
        }

        public void Disable()
        {
            movePad.gameObject.SetActive(false);
            disposables.Clear();
            MoveDirectionAndForce.Value = (Vector2.zero, 0);
        }

        private void EnableMovePad()
        {
            movePad.gameObject.SetActive(true);
            var moveMember = movePadSession.GetMember<MoveMember>();
            movePad.Connect(moveMember)
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

        public void Initialize()
        {
            movePad = inputLayoutService.InputLayout.MovePad;
            movePadSession = new RectBasedSession(movePad.MoveRect);
            fireSession = new RectBasedSession(movePad.MoveRect);
            
            var mouseMoveCompositor = injectCreator.Create<MouseMoveCompositor>();
            movePadSession.AddCompositor(mouseMoveCompositor);
            var mouseSwipeCompositor = injectCreator.Create<MouseSwipeCompositor>(MouseButtonType.Left);
            fireSession.AddCompositor(mouseSwipeCompositor);
        }
    }
}