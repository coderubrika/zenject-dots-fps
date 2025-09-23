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
        private PlayerService playerService;
        
        [SerializeField] private Button closeButton;
        
        private readonly CompositeDisposable disposables = new();
        
        [Inject]
        private void Construct(
            ScreenService screenService,
            EcsService ecsService,
            PlayerService playerService)
        {
            this.screenService = screenService;
            this.ecsService = ecsService;
            this.playerService = playerService;
        }

        public override void Show()
        {
            closeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerService.Clear();
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