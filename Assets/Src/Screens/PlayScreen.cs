using TestRPG.ECS;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TestRPG
{
    public class PlayScreen : BaseScreen
    {
        private ScreenService screenService;
        private EcsService ecsService;
        
        [SerializeField] private Button closeButton;
        
        private readonly CompositeDisposable disposables = new();
        
        [Inject]
        private void Construct(
            ScreenService screenService,
            EcsService ecsService)
        {
            this.screenService = screenService;
            this.ecsService = ecsService;
        }

        public override void Show()
        {
            closeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ecsService.Disable();
                    screenService.GoTo<StartScreen>();
                })
                .AddTo(disposables);
            
            base.Show();
        }
        
        public override void Hide()
        {
            disposables.Clear();
            base.Hide();
        }
    }
}