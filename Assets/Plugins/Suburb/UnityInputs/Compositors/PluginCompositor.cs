using System.Collections.Generic;
using System;
using Suburb.Utils;
using UniRx;

namespace Suburb.Inputs
{
    public abstract class PluginCompositor<TResourceDistributor, TSession, TPlugin> : IPluginCompositor
        where TResourceDistributor : IResourceDistributor
        where TSession : class, ISession
        where TPlugin : IInputPlugin
    {
        protected readonly TResourceDistributor distributor;
        protected readonly LinkedList<TPlugin> plugins = new();
        protected TSession Session { get; private set; }
        
        public IResourceDistributor Distributor => distributor;

        protected PluginCompositor(TResourceDistributor distributor)
        {
            this.distributor = distributor;
        }
        
        public IDisposable Link<TMember>(TPlugin plugin)
            where TMember : class, new()
        {
            if (!plugin.SetReceiver(Session.GetMember<TMember>()))
            {
                this.LogError($"Can't link plugin typeOf '{plugin.GetType().Name}' " +
                              $"because it doesn't support the '{typeof(TMember).Name}' member");
                plugin.Unlink();
                return Disposable.Empty;
            }
            
            if (!plugin.SetSender(this))
            {
                this.LogError($"Can't link plugin typeOf '{plugin.GetType().Name}' " +
                              $"because it doesn't support the '{GetType().Name}' plugin compositor");
                plugin.Unlink();
                return Disposable.Empty;
            }
            
            var node = plugins.AddFirst(plugin);
            return Disposable.Create(() =>
            {
                node.Value.Unlink();
                plugins.Remove(node);
            });
        }

        public abstract void Handle();

        public abstract bool CheckBusy();
        
        public bool SetupSession(ISession session)
        {
            Session = session as TSession;
            return Session != null;
        }

        public virtual void Reset()
        {
            Session = null;
        }
    }
}