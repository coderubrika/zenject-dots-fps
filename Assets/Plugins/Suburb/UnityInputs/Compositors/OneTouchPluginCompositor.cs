using System;
using System.Linq;
using UniRx;

namespace Suburb.Inputs
{
    public class OneTouchPluginCompositor : TouchPluginCompositor
    {
        private IDisposable upDisposable;
        
        public ReactiveProperty<int> Id { get; } = new(-1);
        public int PreviousId { get; private set; } = -1;
        
        public OneTouchPluginCompositor(
            TouchResourceDistributor distributor, 
            TouchProvider touchProvider) : 
            base(distributor, touchProvider)
        {
        }

        public override void Handle()
        {
            base.Handle();

            if (Id.Value != -1)
            {
                HandleFinal();
                return;
            }

            var pointer = ReceivedPointers.FirstOrDefault();

            if (pointer == null)
            {
                HandleFinal();
                return;
            }
            
            if (Session.IsBookResources)
                distributor.SetBookedResource(pointer.Id);
            
            PreviousId = Id.Value;
            Id.Value = pointer.Id;
            upDisposable?.Dispose();
            upDisposable = touchProvider.OnUp
                .Subscribe(_ => UpHandler());
            HandleFinal();
        }

        private void UpHandler()
        {
            HandleUpFinal();
            var pointer = touchProvider.UpEvents
                .FirstOrDefault(item => item.Id == Id.Value);
            
            if (pointer == null)
                return;

            ResetState();
        }
        
        public override bool CheckBusy()
        {
            return Id.Value != -1;
        }

        protected override void ResetState()
        {
            base.ResetState();
            PreviousId = Id.Value;
            Id.Value = -1;
            upDisposable?.Dispose();
        }
    }
}