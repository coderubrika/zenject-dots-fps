using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Suburb.Utils.Serialization
{
    [Serializable]
    public class Vector3Data : IEquatable<Vector3Data>
    {
        [JsonProperty("x")]
        public float X;

        [JsonProperty("y")]
        public float Y;

        [JsonProperty("z")]
        public float Z;

        public bool Equals(Vector3Data other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
