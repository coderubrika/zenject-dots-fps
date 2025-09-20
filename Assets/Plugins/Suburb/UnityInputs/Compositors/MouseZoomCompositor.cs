using System;
using UniRx;

namespace Suburb.Inputs
{
    public class MouseZoomCompositor : BaseMouseCompositor<ZoomMember, IPointerSession>
    {
        public MouseZoomCompositor(MouseProvider mouseProvider, MouseResourceDistributor distributor) : base(mouseProvider, distributor)
        {
        }

        public override void Handle()
        {
            if (!distributor.CheckAvailabilityZoom()
                || !Session.CheckIncludeInBounds(mouseProvider.Position))
                return;
            
            if (Session.IsBookResources)
                distributor.BookZoom();

            Member.PutZoom(mouseProvider.Zoom, mouseProvider.Position);
        }
        
        public override bool CheckBusy()
        {
            return false;
        }
    }
}