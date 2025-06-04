using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Corelib.Utils
{
    public static class ExVector3
    {
        public static void Deconstruct(this Vector3 vec, out float x, out float y, out float z)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }

        public static Vector3Int FloorToInt(this Vector3 vec)
            => new Vector3Int(
                Mathf.FloorToInt(vec.x),
                Mathf.FloorToInt(vec.y),
                Mathf.FloorToInt(vec.z)
            );

        public static Vector3Int CeilToInt(this Vector3 vec)
            => new Vector3Int(
                Mathf.CeilToInt(vec.x),
                Mathf.CeilToInt(vec.y),
                Mathf.CeilToInt(vec.z)
            );
    }
}

