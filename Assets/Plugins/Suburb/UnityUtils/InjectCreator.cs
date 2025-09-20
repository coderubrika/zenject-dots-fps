using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Suburb.Utils
{
    public class InjectCreator
    {
        private readonly DiContainer diContainer;

        public InjectCreator(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
        
        public GameObject Create(GameObject prefab, Transform parent)
        {
            return diContainer.InstantiatePrefab(prefab, parent);
        }
        
        public TComponent Create<TComponent>(GameObject prefab, Transform parent, params object[] args)
            where TComponent : Component
        {
            return diContainer.InstantiatePrefabForComponent<TComponent>(prefab, parent, args);
        }

        public TComponent Create<TComponent>(TComponent component, Transform parent, params object[] args)
            where TComponent : Component
        {
            return diContainer.InstantiatePrefabForComponent<TComponent>(component, parent, args);
        }

        public T Create<T> (params object[] args)
        {
            return diContainer.Instantiate<T>(args);
        }
    }
}
