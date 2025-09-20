using System;
using System.Collections.Generic;

namespace Suburb.Utils
{
    public class GeneralPool<T>
    {
        private readonly Action<T> onSpawn;
        private readonly Action<T> onDespawn;
        private readonly Func<T> instantiate;
        private readonly Queue<T> pool = new();

        public GeneralPool(Action<T> onSpawn, Action<T> onDespawn, Func<T> instantiate)
        {
            this.onSpawn = onSpawn;
            this.onDespawn = onDespawn;
            this.instantiate = instantiate;
        }

        public T Spawn()
        {
            T item = pool.Count > 0 ? pool.Dequeue() : instantiate.Invoke();
            onSpawn.Invoke(item);
            return item;
        }

        public void Despawn(T item)
        {
            onDespawn(item);
            pool.Enqueue(item);
        }
    }
}