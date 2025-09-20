using System;
using Suburb.Common;
using UnityEngine;

namespace Suburb.Utils.Serialization
{
    [Serializable]
    public class PrefabsGroup
    {
        [SerializeField] private string name;
        [SerializeField] private PrefabRef[] prefabs;

        public string Name => name;

        public PrefabRef[] Prefabs => prefabs;
    }
}