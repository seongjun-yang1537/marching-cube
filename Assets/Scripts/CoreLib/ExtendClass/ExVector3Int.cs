using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Corelib.Utils
{
    public static class ExVector3Int
    {
        public static IEnumerable<Vector3Int> Spread(Vector3Int from, Vector3Int to)
        {
            List<Vector3Int> spreads = new();
            for (int x = from.x; x <= to.x; x++)
                for (int y = from.y; y <= to.y; y++)
                    for (int z = from.z; z <= to.z; z++)
                        spreads.Add(new Vector3Int(x, y, z));
            return spreads;
        }

        public static List<int> Flatten(this Vector3Int vec)
            => new List<int>() { vec.x, vec.y, vec.z };

        public static Vector3 ToVector3(this Vector3Int vec)
            => new Vector3(vec.x, vec.y, vec.z);

        public static Vector3Int Min(this Vector3Int vec, Vector3Int other)
            => new Vector3Int(
                Math.Min(vec.x, other.x),
                Math.Min(vec.y, other.y),
                Math.Min(vec.z, other.z)
            );

        public static Vector3Int Max(this Vector3Int vec, Vector3Int other)
            => new Vector3Int(
                Math.Max(vec.x, other.x),
                Math.Max(vec.y, other.y),
                Math.Max(vec.z, other.z)
            );

        public static bool LessEqual(this Vector3Int vec, Vector3Int other)
            => vec.x <= other.x && vec.y <= other.y && vec.z <= other.z;
        public static bool Less(this Vector3Int vec, Vector3Int other)
            => vec.x < other.x && vec.y < other.y && vec.z < other.z;

        public static bool GreaterEqual(this Vector3Int vec, Vector3Int other)
            => vec.x >= other.x && vec.y >= other.y && vec.z >= other.z;
        public static bool Greater(this Vector3Int vec, Vector3Int other)
            => vec.x > other.x && vec.y > other.y && vec.z > other.z;

        public static bool InRange(this Vector3Int vec, Vector3Int l, Vector3Int r)
            => l.GreaterEqual(vec) && vec.LessEqual(r);
    }
}

