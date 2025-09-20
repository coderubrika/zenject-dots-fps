using System;
using UnityEngine;

namespace Suburb.Utils.Serialization
{
    [Serializable]
    public class ValueStartEndAnimationData<TValue> : ValueAnimationData<TValue> 
    {
        [SerializeField] private TValue start;
        
        public TValue Start => start;
    }
}