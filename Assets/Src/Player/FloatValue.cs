using System;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace TestRPG.PlayerDir
{
    public class FloatValue
    {
        private MinMax bounds;
        private readonly ReactiveProperty<float> value = new();
        public float Value => value.Value;

        public IObservable<float> OnChange => value;

        public FloatValue(float initialValue, float max)
        {
            bounds = new MinMax(0, Mathf.Clamp(max, float.Epsilon, float.MaxValue));
            value.Value = bounds.Clamp(initialValue);
        }
        
        public void SetValue(float newValue)
        {
            value.Value = bounds.Clamp(newValue);
        }

        public void ChangeMax(float max)
        {
            bounds.Max = Mathf.Clamp(max, float.Epsilon, float.MaxValue);
        }

        public float GetPercentage()
        {
            return bounds.InverseLerp(Value);
        }
    }
}