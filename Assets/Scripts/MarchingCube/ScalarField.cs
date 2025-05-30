using System;
using System.Collections.Generic;
using UnityEngine;

namespace MCube
{
    [CreateAssetMenu(fileName = "New Scalar Field", menuName = "ScriptableObject/Scalar Field")]
    public class ScalarField : ScriptableObject
    {
        private const long LIMIT_SIZE = 1_000_000;

        public Vector3Int size;
        public float[] field;

        public void Resize(Vector3Int newSize)
        {
            if (IsLimitSize(newSize))
            {
                Debug.LogWarning($"Requested size {newSize} exceeds limit ({LIMIT_SIZE}). Resize aborted.");
                return;
            }

            float[] newField = new float[newSize.x * newSize.y * newSize.z];

            if (field != null)
            {
                int minX = Mathf.Min(size.x, newSize.x);
                int minY = Mathf.Min(size.y, newSize.y);
                int minZ = Mathf.Min(size.z, newSize.z);

                for (int x = 0; x < minX; x++)
                {
                    for (int y = 0; y < minY; y++)
                    {
                        for (int z = 0; z < minZ; z++)
                        {
                            int oldIndex = GetIndex(x, y, z, size);
                            int newIndex = GetIndex(x, y, z, newSize);
                            newField[newIndex] = field[oldIndex];
                        }
                    }
                }
            }

            size = newSize;
            field = newField;
        }

        public bool IsLimitSize(Vector3Int newSize)
        {
            long magnitude = (long)newSize.x * newSize.y * newSize.z;
            return magnitude > LIMIT_SIZE;
        }

        public void GenerateRandom()
        {
            uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
            MT19937 mt = new MT19937(seed);

            foreach (Vector3Int i in Index())
            {
                this[i] = mt.NextFloat();
            }
        }

        public IEnumerable<Vector3Int> Index()
        {
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    for (int z = 0; z < size.z; z++)
                        yield return new Vector3Int(x, y, z);
        }

        private static int GetIndex(int x, int y, int z, Vector3Int size)
        {
            return x + size.x * (y + size.y * z);
        }

        public float this[int x, int y, int z]
        {
            get => field[GetIndex(x, y, z, size)];
            set => field[GetIndex(x, y, z, size)] = value;
        }

        public float this[Vector3Int coord]
        {
            get => this[coord.x, coord.y, coord.z];
            set => this[coord.x, coord.y, coord.z] = value;
        }
    }
}