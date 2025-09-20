using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Suburb.Utils
{
    public static class MathUtils
    {
        public const float GRAVITATION_CONST = 6.67430e-11f;
        
        public static Vector3 FindDestination(Vector3 sourcePosition, Vector3 forward, float distance)
        {
            return sourcePosition + forward * distance;
        }

        public static Vector3 RandVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z));
        }
    }
}
