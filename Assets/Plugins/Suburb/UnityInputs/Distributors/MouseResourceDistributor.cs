using System;
using System.Linq;
using UniRx;
using Suburb.Utils;

namespace Suburb.Inputs
{
    public class MouseResourceDistributor : IResourceDistributor
    {
        private readonly MouseProvider mouseProvider;
        
        private readonly CompositeDisposable disposables = new();
        private readonly bool[] availableButtons = new bool[3];

        private bool isAvailableZoom;
        private bool isAvailableMove;
        private int usersCount;
        
        public ReactiveCommand OnAppearResources { get; } = new();
        public bool HaveResources => availableButtons.Any(isAvailable => isAvailable) || isAvailableZoom || isAvailableMove;

        public MouseResourceDistributor(MouseProvider mouseProvider)
        {
            this.mouseProvider = mouseProvider;
        }

        public bool CheckAvailabilityButton(MouseButtonType buttonType) => availableButtons[(int)buttonType];
        
        public void SetBookedButton(MouseButtonType buttonType) => availableButtons[(int)buttonType] = false;
        
        public bool CheckAvailabilityZoom() => isAvailableZoom;
        
        public void BookZoom()
        {
            isAvailableZoom = false;
        }
        
        public bool CheckAvailabilityMove() => isAvailableMove;

        public void BookMove()
        {
            isAvailableMove = false;
        } 
        
        public IDisposable Enable()
        {
            if (usersCount == 0)
            {
                mouseProvider.Enable()
                    .AddTo(disposables);

                mouseProvider.OnDown
                    .Subscribe(buttonType =>
                    {
                        availableButtons[(int)buttonType] = true;
                        OnAppearResources.Execute();
                    })
                    .AddTo(disposables);
                
                mouseProvider.OnUp
                    .Subscribe(buttonType => availableButtons[(int)buttonType] = false)
                    .AddTo(disposables);
                
                mouseProvider.OnZoom
                    .Subscribe(_ =>
                    {
                        isAvailableZoom = true;
                        OnAppearResources.Execute();
                    })
                    .AddTo(disposables);
                
                mouseProvider.OnMove
                    .Subscribe(_ =>
                    {
                        isAvailableMove = true;
                        OnAppearResources.Execute();
                    })
                    .AddTo(disposables);
            }

            usersCount += 1;
            return Disposable.Create(Disable);
        }

        private void Disable()
        {
            if (usersCount == 0)
                return;
            
            usersCount -= 1;

            if (usersCount > 0)
                return;
            
            availableButtons.Fill(false);
            isAvailableZoom = false;
            isAvailableMove = false;
            disposables.Clear();
        }
    }
}