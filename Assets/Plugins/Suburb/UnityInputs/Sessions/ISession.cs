using UniRx;

namespace Suburb.Inputs
{
    public interface ISession
    {
        public IResourceDistributor[] GetResourceDistributors();
        public void HandleResources(IResourceDistributor distributor);

        public bool IsBookResources { get; }
        public bool IsPreventNext { get; }
        
        public ReactiveCommand<IResourceDistributor> OnDistributorAdded { get; }
        public ReactiveCommand<IResourceDistributor> OnDistributorRemoved { get; }
        
        public TMember GetMember<TMember>()
            where TMember : class, new();
    }
}