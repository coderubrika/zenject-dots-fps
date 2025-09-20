using System.Collections.Generic;
using System.Linq;
using Suburb.Utils;

namespace Suburb.Inputs
{
    public abstract class TouchPluginCompositor : PluginCompositor<TouchResourceDistributor, IPointerSession, IInputPlugin>
    {
        protected readonly TouchProvider touchProvider;
        
        private readonly List<int> reservedTouchIds = new();
        protected HashSet<PointerEventData> ReceivedPointers { get; } = new();
        protected HashSet<PointerEventData> UpPointers { get; } = new();
        
        public TouchPluginCompositor(
            TouchResourceDistributor distributor,
            TouchProvider touchProvider) : base(distributor)
        {
            this.touchProvider = touchProvider;
        }

        public override void Handle()
        {
            ReceivedPointers.Clear();
            ReceivedPointers.AddRange(distributor
                .GetAvailableResources().Where(pointer => Session.CheckIncludeInBounds(pointer.Position)));
            
            if (Session.IsPreventNext)
                distributor.SetBookedResources(ReceivedPointers
                    .Select(item => item.Id)
                    .ToArray());
        }

        public override void Reset()
        {
            base.Reset();
            ResetState();
        }

        protected virtual void ResetState()
        {
            ReceivedPointers.Clear();
            reservedTouchIds.Clear();
            UpPointers.Clear();
        }
        
        protected void HandleFinal()
        {
            if (!Session.IsPreventNext)
                return;
            reservedTouchIds.AddRange(ReceivedPointers.Select(item => item.Id));
        }
        
        protected void HandleUpFinal()
        {
            foreach (var pointer in touchProvider.UpEvents)
            {
                UpPointers.Add(pointer);
                reservedTouchIds.Remove(pointer.Id);
            }
        }
    }
}