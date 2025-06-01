using System.Collections.Generic;
using UnityEngine;
namespace Corelib.Utils
{
    public static class ExList
    {
        public static void Swap<T>(this List<T> list, int idx1, int idx2)
        {
            (list[idx1], list[idx2]) = (list[idx2], list[idx1]);
        }

        public static void Resize<T>(this List<T> list, int size, T value)
        {
            List<T> copy = new List<T>(list);
            list.Clear();
            for (int i = 0; i < size; i++)
            {
                if (i < copy.Count)
                {
                    list.Add(copy[i]);
                }
                else
                {
                    list.Add(value);
                }
            }
        }

        public static void Resize<T>(this List<T> list, int size)
            => list.Resize(size, default);
    }
}