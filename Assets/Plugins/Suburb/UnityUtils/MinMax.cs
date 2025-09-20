using System;

namespace Suburb.Utils
{
    [Serializable]
    public struct MinMax
    {
        public float Min;
        public float Max;
        
        public MinMax(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}