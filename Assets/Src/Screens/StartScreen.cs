using TestRPG.ECS;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TestRPG
{
    public class StartScreen : BaseScreen
    {
        [SerializeField] private Button playButton;
        
        private ScreenService screenService;
        private EcsService ecsService;
        private LoadingModalService loadingModalService;
        private PlayerService playerService;
        
        private readonly CompositeDisposable disposables = new();
        
        [Inject]
        private void Construct(
            ScreenService screenService,
            EcsService ecsService,
            LoadingModalService loadingModalService,
            PlayerService playerService)
        {
            this.screenService = screenService;
            this.ecsService = ecsService;
            this.loadingModalService = loadingModalService;
            this.playerService = playerService;
        }

        public override void Show()
        {
            playButton.OnClickAsObservable()
                .Subscribe(_ => LoadGameScene())
                .AddTo(disposables);
            
            base.Show();
        }

        public override void Hide()
        {
            disposables.Clear();
            base.Hide();
        }

        private void LoadGameScene()
        {
            loadingModalService.Show();
            ecsService.Enable()
                .Subscribe(_ =>
                {
                    playerService.SetupPlayer();
                    loadingModalService.Hide();
                    screenService.GoTo<PlayScreen>();
                }, _ => loadingModalService.Hide())
                .AddTo(disposables);
        }
    }
}