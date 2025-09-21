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
        
        private readonly CompositeDisposable disposables = new();
        
        [Inject]
        private void Construct(
            ScreenService screenService,
            EcsService ecsService,
            LoadingModalService loadingModalService)
        {
            this.screenService = screenService;
            this.ecsService = ecsService;
            this.loadingModalService = loadingModalService;
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
                    loadingModalService.Hide();
                    screenService.GoTo<PlayScreen>();
                }, _ => loadingModalService.Hide())
                .AddTo(disposables);
        }
    }
}