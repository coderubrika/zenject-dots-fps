using System.Linq;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class OneTwoTouchPluginCompositor : TouchPluginCompositor
    {
        private readonly CompositeDisposable disposables = new();
        
        public int TouchCount { get; private set; }
        public int PreviousFirst { get; private set; } = -1;
        public int PreviousSecond { get; private set; } = -1;
        public ReactiveProperty<int> First { get; } = new(-1);
        public ReactiveProperty<int> Second { get; } = new(-1);
        public ReactiveCommand OnBothBegin { get; } = new();
        public ReactiveCommand OnBothEnd { get; } = new();
        
        public OneTwoTouchPluginCompositor(
            TouchProvider touchProvider,
            TouchResourceDistributor distributor) 
            : base(distributor, touchProvider)
        {
        }

        public override void Handle()
        {
            var resources = ReceivedPointers
                .Take(Mathf.Clamp(2 - TouchCount, 0, 2))
                .Select(item => item.Id)
                .ToArray();

            if (resources.Length == 0)
                return;

            if (Session.IsPreventNext || Session.IsBookResources)
                distributor.SetBookedResources(resources);

            TouchCount += resources.Length;
            
            if (resources.Length == 2)
            {
                touchProvider.OnUp
                    .Subscribe(_ => UpHandler())
                    .AddTo(disposables);
                
                OnBothBegin.Execute();
                
                PreviousFirst = First.Value;
                First.Value = resources[0];

                PreviousSecond = Second.Value;
                Second.Value = resources[1];
                
                OnBothEnd.Execute();
                return;
            }

            if (First.Value == -1)
            {
                touchProvider.OnUp
                    .Subscribe(_ => UpHandler())
                    .AddTo(disposables);
                
                PreviousFirst = First.Value;
                First.Value = resources[0];
            }
            else
            {
                PreviousSecond = Second.Value;
                Second.Value = resources[0];
            }
        }

        public override bool CheckBusy()
        {
            return TouchCount == 2;
        }

        protected override void ResetState()
        {
            base.ResetState();
            PreviousFirst = First.Value;
            PreviousSecond = -1;
            Second.Value = -1;
            First.Value = -1;
            TouchCount = 0;
            disposables.Clear();
        }
        
        private void UpHandler()
        {
            HandleUpFinal();
            var pointers = UpPointers
                .Where(item => item.Id == First.Value || item.Id == Second.Value).ToArray();
            
            if (pointers.Length == 0)
                return;

            if (pointers.Length == 1)
            {
                if (TouchCount == 2)
                {
                    PreviousSecond = Second.Value;

                    if (First.Value == pointers[0].Id)
                    {
                        PreviousFirst = First.Value;
                        First.Value = Second.Value;
                    }

                    Second.Value = -1;
                    TouchCount = 1;
                }
                else
                    ResetState();
            }
            else
                ResetState();
        }
    }
}