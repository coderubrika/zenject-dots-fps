using System;
using System.Collections.Generic;
using System.Linq;
using Suburb.Utils;
using UniRx;

namespace Suburb.Inputs
{
    public class TouchResourceDistributor : IResourceDistributor
    {
        private readonly TouchProvider touchProvider;
        private readonly CompositeDisposable disposables = new();
        private readonly PointerEventData[] availableResources;
        
        private int usersCount;

        public ReactiveCommand OnAppearResources { get; } = new();
        public bool HaveResources => availableResources.Any(item => item != null);

        public TouchResourceDistributor(TouchProvider touchProvider)
        {
            this.touchProvider = touchProvider;
            availableResources = new PointerEventData[touchProvider.SupportedTouches];
        }
        
        public IEnumerable<PointerEventData> GetAvailableResources()
        {
            return availableResources
                .FilterNull();
        }

        public void SetBookedResources(IEnumerable<int> ids)
        {
            foreach (int id in ids)
                availableResources[id] = null;
        }
        
        public void SetBookedResource(int id)
        {
            availableResources[id] = null;
        }

        public IDisposable Enable()
        {
            if (usersCount == 0)
            {
                touchProvider.Enable()
                    .AddTo(disposables);

                touchProvider.OnDown
                    .Subscribe(_ =>
                    {
                        availableResources.Fill(null);
                        foreach (var appearEvent in touchProvider.DownEvents)
                            availableResources[appearEvent.Id] = appearEvent;
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
            
            availableResources.Fill(null);
            disposables.Clear();
        }
    }
}