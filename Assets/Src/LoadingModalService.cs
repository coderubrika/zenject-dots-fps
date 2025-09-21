using Suburb.Utils;
using Zenject;

namespace TestRPG
{
    public class LoadingModalService : IInitializable
    {
        private readonly InjectCreator injectCreator;
        private readonly LoadingModal loadingModalPrefab;
        private readonly ModalRoot modalRoot;

        private LoadingModal loadingModal;
        
        public LoadingModalService(
            InjectCreator injectCreator,
            ModalRoot modalRoot,
            LoadingModal loadingModalPrefab)
        {
            this.injectCreator = injectCreator;
            this.modalRoot = modalRoot;
            this.loadingModalPrefab = loadingModalPrefab;
        }
        
        public void Initialize()
        {
            loadingModal = injectCreator.Create(loadingModalPrefab, modalRoot.Root);
            loadingModal.gameObject.SetActive(false);
        }

        public void Show()
        {
            loadingModal.gameObject.SetActive(true);
        }

        public void Hide()
        {
            loadingModal.gameObject.SetActive(false);
        }
    }
}