namespace Suburb.Inputs
{
    public abstract class BaseMouseCompositor<TMember, TSession> : IPluginCompositor
        where TMember : class, new()
        where TSession : class, ISession
    {
        protected readonly MouseProvider mouseProvider;
        protected readonly MouseResourceDistributor distributor;
        
        public BaseMouseCompositor(MouseProvider mouseProvider, MouseResourceDistributor distributor)
        {
            this.mouseProvider = mouseProvider;
            this.distributor = distributor;
        }
        
        protected TSession Session {get; private set;}
        protected TMember Member {get; private set;}
        public IResourceDistributor Distributor => distributor;

        public abstract void Handle();

        public abstract bool CheckBusy();

        public bool SetupSession(ISession session)
        {
            Session = session as TSession;

            if (session != null)
                Member = session.GetMember<TMember>();
            
            return Session != null;
        }

        public virtual void Reset()
        {
            Session = null;
            Member = null;
        }
    }
}