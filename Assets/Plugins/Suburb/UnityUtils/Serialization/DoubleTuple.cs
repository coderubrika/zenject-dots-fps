using System;
using UnityEngine;

namespace Suburb.Utils.Serialization
{
    [Serializable]
    public class DoubleTuple<TKey, TValue>
    {
        [SerializeField] private TKey key;
        [SerializeField] private TValue value;
        
        public TKey Key => key;
        public TValue Value => value;
    }
}