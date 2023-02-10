using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class MiscExtensions
    {
        public static float RandomWithin(this Vector2 minMaxVector)
        {
            return Random.Range(minMaxVector.x, minMaxVector.y);
        }

        public static T GetRandom<T>(this IReadOnlyList<T> collection)
        {
            if (collection == null || collection.Count == 0) return default;
            var index = Random.Range(0, collection.Count);
            return collection[index];
        }
    }
}