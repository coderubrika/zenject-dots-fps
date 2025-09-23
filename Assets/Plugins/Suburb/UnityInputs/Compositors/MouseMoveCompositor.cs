namespace Suburb.Inputs
{
    public class MouseMoveCompositor : BaseMouseCompositor<MoveMember, IPointerSession>
    {
        public MouseMoveCompositor(MouseProvider mouseProvider, MouseResourceDistributor distributor) : base(mouseProvider, distributor)
        {
        }
        
        public override void Handle()
        {
            if (!distributor.CheckAvailabilityMove()
                || !Session.CheckIncludeInBounds(mouseProvider.Position))
                return;
            
            if (Session.IsBookResources)
                distributor.BookMove();

            Member.PutMove(mouseProvider.Delta);
        }
        
        public override bool CheckBusy()
        {
            return false;
        }
    }
}