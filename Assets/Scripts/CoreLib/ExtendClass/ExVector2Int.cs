using UnityEngine;

namespace Corelib.Utils
{
    public static class ExVector2Int
    {
        public static void Deconstruct(this Vector2Int vec, out int x, out int y)
        {
            x = vec.x;
            y = vec.y;
        }
    }
}