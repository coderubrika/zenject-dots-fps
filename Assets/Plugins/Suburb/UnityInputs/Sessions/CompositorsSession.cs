using System;
using System.Collections.Generic;
using System.Linq;
using Suburb.Utils;
using UniRx;

namespace Suburb.Inputs
{
    public class CompositorsSession : BaseSession
    {
        private readonly Dictionary<IResourceDistributor, LinkedList<IPluginCompositor>> compositorsStore = new();
        
        public IDisposable AddCompositor(IPluginCompositor compositor)
        {
            if (!compositor.SetupSession(this))
            {
                this.Log($"{compositor.GetType().Name} failed to setup with {GetType().Name}");
                return Disposable.Empty;
            }
            
            if (compositorsStore.TryGetValue(compositor.Distributor, out var compositors))
            {
                var node = compositors.AddFirst(compositor);
                return Disposable.Create(() =>
                {
                    compositor.Reset();
                    compositors.Remove(node);
                    if (compositors.Count == 0)
                    {
                        compositorsStore.Remove(compositor.Distributor);
                        OnDistributorRemoved.Execute(compositor.Distributor);
                    }
                });
            }

            var newCompositors = new LinkedList<IPluginCompositor>();
            var newNode = newCompositors.AddFirst(compositor);
            compositorsStore.Add(compositor.Distributor, newCompositors);
            OnDistributorAdded.Execute(compositor.Distributor);
            
            return Disposable.Create(() =>
            {
                compositor.Reset();
                newCompositors.Remove(newNode);
                if (newCompositors.Count == 0)
                {
                    compositorsStore.Remove(compositor.Distributor);
                    OnDistributorRemoved.Execute(compositor.Distributor);
                }
            });
        }
        
        public override IResourceDistributor[] GetResourceDistributors()
        {
            return compositorsStore.Keys.ToArray();
        }

        public override void HandleResources(IResourceDistributor distributor)
        {
            var compositors = compositorsStore[distributor];
            foreach (var compositor in compositors.Where(compositor => !compositor.CheckBusy() || IsPreventNext))
                compositor.Handle();
        }
    }
}