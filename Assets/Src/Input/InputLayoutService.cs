using Suburb.Utils;
using Zenject;

namespace TestRPG.Input
{
    public class InputLayoutService : IInitializable
    {
        private readonly ModalRoot modalRoot;
        private readonly InjectCreator injectCreator;
        private readonly InputLayout inputLayoutPrefab;

        public InputLayout InputLayout { get; private set; }
        
        public InputLayoutService(
            ModalRoot modalRoot,
            InjectCreator injectCreator,
            InputLayout inputLayoutPrefab)
        {
            this.modalRoot = modalRoot;
            this.injectCreator = injectCreator;
            this.inputLayoutPrefab = inputLayoutPrefab;
        }
        
        public void Initialize()
        {
            InputLayout = injectCreator.Create(inputLayoutPrefab, modalRoot.Root);
            
            InputLayout.MovePad.gameObject.SetActive(false);
            InputLayout.TouchPad.gameObject.SetActive(false);
            InputLayout.StickZone.gameObject.SetActive(false);
        }
    }
}