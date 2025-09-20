using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Suburb.Utils.Serialization
{
    [Serializable]
    public class TransformData
    {
        [JsonProperty("position")]
        public Vector3 Position;
        
        [JsonProperty("rotation")]
        public Vector3 Rotation;
        
        [JsonProperty("scale")]
        public Vector3 Scale;
    }
}