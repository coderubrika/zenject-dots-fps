using TestRPG.ECS;
using TestRPG.PlayerDir;
using TestRPG.UI;
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
        [SerializeField] private SliderView healthBar;
        
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
            playerService.OnGameState
                .Where(state => state == GameStateVariant.Stop)
                .Subscribe(_ =>
                {
                    playerService.Clear();
                    ecsService.Disable();
                    screenService.GoTo<StartScreen>();
                })
                .AddTo(disposables);
            
            closeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerService.Clear();
                    ecsService.Disable();
                    screenService.GoTo<StartScreen>();
                })
                .AddTo(disposables);
            
            healthBar.SetValue(playerService.PlayerData.Health.Value);

            playerService.PlayerData.Health.OnChange
                .Subscribe(_ => healthBar.SetValue(playerService.PlayerData.Health.GetPercentage()))
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