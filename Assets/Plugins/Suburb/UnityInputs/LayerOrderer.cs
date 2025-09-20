using System;
using System.Collections.Generic;
using Suburb.Utils;
using UniRx;

namespace Suburb.Inputs
{
    public class LayerOrderer
    {
        private readonly HashSet<ISession> sessionsStore = new();
        private readonly Dictionary<IResourceDistributor, LinkedList<ISession>> distributorsSessionsStore = new();
        private readonly Dictionary<IResourceDistributor, (IDisposable Subscription, IDisposable Enabling)> distributorSubscriptions = new();

        public IDisposable ConnectFirst(ISession session)
        {
            if (!sessionsStore.Add(session))
            {
                this.LogError("Session already connected");
                return Disposable.Empty;
            }

            IDisposable addDisposable = session.OnDistributorAdded
                .Subscribe(distributor =>
                {
                    GetSessionsByDistributorFromSession(distributor)
                        .AddFirst(session);
                });
            
            IDisposable removeDisposable = session.OnDistributorRemoved
                .Subscribe(distributor => RemoveDistributorFromSession(session, distributor));
            
            foreach (var distributor in session.GetResourceDistributors())
                GetSessionsByDistributorFromSession(distributor)
                    .AddFirst(session);
            
            return Disposable.Create(() =>
            {
                addDisposable.Dispose();
                removeDisposable.Dispose();
                sessionsStore.Remove(session);
                foreach (var distributor in session.GetResourceDistributors())
                    RemoveDistributorFromSession(session, distributor);
            });
        }
        
        public IDisposable ConnectLast(ISession session)
        {
            if (!sessionsStore.Add(session))
            {
                this.LogError("Session already connected");
                return Disposable.Empty;
            }

            IDisposable addDisposable = session.OnDistributorAdded
                .Subscribe(distributor =>
                {
                    GetSessionsByDistributorFromSession(distributor)
                        .AddFirst(session);
                });
            
            IDisposable removeDisposable = session.OnDistributorRemoved
                .Subscribe(distributor => RemoveDistributorFromSession(session, distributor));
            
            foreach (var distributor in session.GetResourceDistributors())
                GetSessionsByDistributorFromSession(distributor)
                    .AddLast(session);
            
            return Disposable.Create(() =>
            {
                addDisposable.Dispose();
                removeDisposable.Dispose();
                sessionsStore.Remove(session);
                foreach (var distributor in session.GetResourceDistributors())
                    RemoveDistributorFromSession(session, distributor);
            });
        }
        
        public IDisposable ConnectBefore(ISession sessionOrigin, ISession sessionTarget)
        {
            if (!sessionsStore.Add(sessionTarget))
            {
                this.LogError("Session already connected");
                return Disposable.Empty;
            }

            if (!sessionsStore.Contains(sessionOrigin))
            {
                this.LogError("Session origin not connected");
                return Disposable.Empty;
            }
            
            IDisposable addDisposable = sessionTarget.OnDistributorAdded
                .Subscribe(distributor =>
                {
                    GetSessionsByDistributorFromSession(distributor)
                        .AddFirst(sessionTarget);
                });
            
            IDisposable removeDisposable = sessionTarget.OnDistributorRemoved
                .Subscribe(distributor => RemoveDistributorFromSession(sessionTarget, distributor));

            foreach (var distributor in sessionTarget.GetResourceDistributors())
            {
                var sessionsList = GetSessionsByDistributorFromSession(distributor);
                var originNode = sessionsList.Find(sessionOrigin);

                if (originNode == null)
                    sessionsList.AddFirst(sessionTarget);
                else
                    sessionsList.AddBefore(originNode, sessionTarget);
            }
            
            return Disposable.Create(() =>
            {
                addDisposable.Dispose();
                removeDisposable.Dispose();
                sessionsStore.Remove(sessionTarget);
                foreach (var distributor in sessionTarget.GetResourceDistributors())
                    RemoveDistributorFromSession(sessionTarget, distributor);
            });
        }
        
        public IDisposable ConnectAfter(ISession sessionOrigin, ISession sessionTarget)
        {
            if (!sessionsStore.Add(sessionTarget))
            {
                this.LogError("Session already connected");
                return Disposable.Empty;
            }

            if (!sessionsStore.Contains(sessionOrigin))
            {
                this.LogError("Session origin not connected");
                return Disposable.Empty;
            }
            
            IDisposable addDisposable = sessionTarget.OnDistributorAdded
                .Subscribe(distributor =>
                {
                    GetSessionsByDistributorFromSession(distributor)
                        .AddFirst(sessionTarget);
                });
            
            IDisposable removeDisposable = sessionTarget.OnDistributorRemoved
                .Subscribe(distributor => RemoveDistributorFromSession(sessionTarget, distributor));

            foreach (var distributor in sessionTarget.GetResourceDistributors())
            {
                var sessionsList = GetSessionsByDistributorFromSession(distributor);
                var originNode = sessionsList.Find(sessionOrigin);

                if (originNode == null)
                    sessionsList.AddFirst(sessionTarget);
                else
                    sessionsList.AddAfter(originNode, sessionTarget);
            }
            
            return Disposable.Create(() =>
            {
                addDisposable.Dispose();
                removeDisposable.Dispose();
                sessionsStore.Remove(sessionTarget);
                foreach (var distributor in sessionTarget.GetResourceDistributors())
                    RemoveDistributorFromSession(sessionTarget, distributor);
            });
        }
        
        private LinkedList<ISession> GetSessionsByDistributorFromSession(IResourceDistributor distributor)
        {
            if (distributorsSessionsStore.TryGetValue(distributor, out var sessionsInDistributor))
                return sessionsInDistributor;
            var newList = new LinkedList<ISession>();
            distributorsSessionsStore.Add(distributor, newList);
            
            if (!distributorSubscriptions.ContainsKey(distributor))
            {
                IDisposable disposable = distributor.OnAppearResources
                    .Subscribe(_ => HandleSessions(distributor));
                distributorSubscriptions.Add(distributor, (disposable, distributor.Enable()));
            }

            return newList;
        }

        private void RemoveDistributorFromSession(ISession session, IResourceDistributor distributor)
        {
            var sessionsInDistributor = distributorsSessionsStore[distributor];
            sessionsInDistributor.Remove(session);
            if (sessionsInDistributor.Count > 0)
                return;
                    
            distributorsSessionsStore.Remove(distributor);
            var disposables = distributorSubscriptions[distributor];
            disposables.Subscription.Dispose();
            disposables.Enabling.Dispose();
            distributorSubscriptions.Remove(distributor);
        }

        private void HandleSessions(IResourceDistributor distributor)
        {
            LinkedList<ISession> sessions = distributorsSessionsStore[distributor];
            var nextNode = sessions.First;
            while (nextNode != null)
            {
                var session = nextNode.Value;
                if (!distributor.HaveResources)
                    break;
                
                session.HandleResources(distributor);

                if (nextNode.List == null)
                {
                    if (distributorsSessionsStore.TryGetValue(distributor, out sessions))
                    {
                        nextNode = sessions.First;
                        continue;
                    }
                    break;
                }
                
                nextNode = nextNode.Next;
            }
        }
    }
}