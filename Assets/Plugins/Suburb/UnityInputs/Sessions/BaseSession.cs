using System;
using System.Collections.Generic;
using UniRx;

namespace Suburb.Inputs
{
    public abstract class BaseSession : ISession
    {
        private readonly Dictionary<Type, object> members = new();
        
        public bool IsBookResources { get; private set; }

        public bool IsPreventNext { get; private set; }

        public ReactiveCommand<IResourceDistributor> OnDistributorAdded { get; } = new();
        
        public ReactiveCommand<IResourceDistributor> OnDistributorRemoved { get; } = new();

        public abstract IResourceDistributor[] GetResourceDistributors();

        public abstract void HandleResources(IResourceDistributor distributor);
        
        public void SetBookResources(bool isBook)
        {
            IsBookResources = isBook;
        }
        
        public void SetPreventNext(bool isOn)
        {
            IsPreventNext = isOn;
        }

        public TMember GetMember<TMember>() 
            where TMember : class, new()
        {
            Type memberType = typeof(TMember);
            if (members.TryGetValue(memberType, out object member))
                return member as TMember;

            var newMember = new TMember();
            members.Add(memberType, newMember);
            return newMember;
        }
    }
}