using System.Collections.Generic;
using System.Linq;
using Suburb.Utils.Serialization;
using UnityEngine;

namespace Suburb.Common
{
    public class PrefabRef : MonoBehaviour
    {
        [SerializeField] private DoubleTuple<string, Object>[] refs;
        public Dictionary<string, Object> Refs => refs.ToDictionary(item => item.Key, item => item.Value);
    }
}