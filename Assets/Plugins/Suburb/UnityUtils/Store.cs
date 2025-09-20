using System.Collections;
using System.Collections.Generic;

namespace Suburb.Utils
{
    public class Store<TItem> : IEnumerable<TItem>
    {
        private readonly Queue<int> freeIndexes = new();
        private readonly Dictionary<TItem, int> itemKeys = new();
        private readonly List<TItem> items = new();

        public bool Push(TItem item)
        {
            if (item == null || itemKeys.ContainsKey(item))
                return false;
            
            int idx;
            if (freeIndexes.Count > 0)
            {
                idx = freeIndexes.Dequeue();
                items[idx] = item;
            }
            else
            {
                idx = items.Count;
                items.Add(item);
            }
            
            SetKey(idx, item);
            return true;
        }

        public bool Remove(TItem item)
        {
            if (!itemKeys.Remove(item, out int idx)) 
                return false;
            
            items[idx] = default;
            freeIndexes.Enqueue(idx);
            return true;
        }

        public void Clear()
        {
            itemKeys.Clear();
            items.Clear();
            freeIndexes.Clear();
        }
        
        private void SetKey(int key, TItem item)
        {
            itemKeys.Add(item, key);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return new StoreEnumerator(items);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class StoreEnumerator : IEnumerator<TItem>
        {
            private readonly List<TItem> items;
            private int index = -1;
            
            public StoreEnumerator(List<TItem> items)
            {
                this.items = items;
            }
            
            public bool MoveNext()
            {
                while (++index < items.Count && items[index] == null) {}
                return index < items.Count;
            }

            public void Reset()
            {
                index = -1;
            }

            public TItem Current => items[index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }
        }
    }
}