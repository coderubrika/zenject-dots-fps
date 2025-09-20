using UniRx;

namespace Suburb.Inputs
{
    public class MouseSwipeCompositor : BaseMouseCompositor<SwipeMember, IPointerSession>
    {
        private readonly MouseButtonType buttonType;
        
        private readonly CompositeDisposable disposables = new();
        
        private GestureType gestureType = GestureType.None;

        public MouseSwipeCompositor(MouseProvider mouseProvider, MouseResourceDistributor distributor, MouseButtonType buttonType) 
            : base(mouseProvider, distributor)
        {
            this.buttonType = buttonType;
        }

        public override void Handle()
        {
            if (!distributor.CheckAvailabilityButton(buttonType)
                || !Session.CheckIncludeInBounds(mouseProvider.Position))
                return;
            
            mouseProvider.OnMove
                .Subscribe(_ =>
                {
                    if (gestureType == GestureType.Down)
                    {
                        Member.PutDragStart(mouseProvider.Delta);
                        gestureType = GestureType.Drag;
                        return;
                    }
                    
                    if (gestureType != GestureType.Drag)
                        return;
                    
                    Member.PutDrag(mouseProvider.Delta);
                })
                .AddTo(disposables);

            mouseProvider.OnUp
                .Where(type => type == buttonType)
                .Subscribe(_ =>
                {
                    if (gestureType == GestureType.Drag)
                        Member.PutDragEnd();
                    
                    gestureType = GestureType.None;
                    Member.PutUp(mouseProvider.Position);
                    disposables.Clear();
                })
                .AddTo(disposables);
            
            if (Session.IsBookResources)
                distributor.SetBookedButton(buttonType);
            
            gestureType = GestureType.Down;
            Member.PutDown(mouseProvider.Position);
        }

        public override bool CheckBusy()
        {
            return gestureType != GestureType.None;
        }
    }
}