using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Suburb.Utils
{
    public static class GeneralUtils
    {
        public static IObservable<T> StartWithDefault<T> (T defaultValue = default)
        {
            return Observable.Start(() => defaultValue);
        }

        public static string GetUID()
        {
            return Guid.NewGuid().ToString();
        }
        
        public static IEnumerable<T> Generate<T>(Func<int, T> generator, int count)
        {
            T[] collection = new T[count];
            
            for (int i = 0; i < collection.Length; i++)
                collection[i] = generator.Invoke(i);

            return collection.AsEnumerable();
        }
    }
}
