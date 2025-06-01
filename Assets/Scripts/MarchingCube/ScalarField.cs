using System;
using System.Collections.Generic;
using Corelib.Utils;
using Sirenix.Utilities;
using UnityEngine;

namespace MCube
{
    [CreateAssetMenu(fileName = "New Scalar Field", menuName = "ScriptableObject/Scalar Field")]
    public class ScalarField : ScriptableObject
    {
        private const long LIMIT_SIZE = 1_000_000;

        public Vector3Int size;
        public float[] field;

        public IEnumerable<Vector3Int> Indices { get => CIterator.GetArray3D(size); }

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
                Vector3Int minSize = size.Min(newSize);
                CIterator.GetArray3D(minSize)
                    .ForEach(
                        idx => newField[GetIndex(idx, newSize)] = field[GetIndex(idx, size)]
                    );
            }
            (size, field) = (newSize, newField);
        }

        public bool IsLimitSize(Vector3Int newSize)
        {
            long magnitude = (long)newSize.x * newSize.y * newSize.z;
            return magnitude > LIMIT_SIZE;
        }

        public void ClearField() => Indices.ForEach(idx => this[idx] = 0.0f);

        public int GetIndex(Vector3Int pos) => GetIndex(pos.x, pos.y, pos.z);
        public int GetIndex(int x, int y, int z) => GetIndex(x, y, z, size);

        public Vector3Int SpreadIndex(int idx)
        {
            int xy = size.x * size.y;
            int z = idx / xy;
            int rem = idx % xy;
            int y = rem / size.x;
            int x = rem % size.x;
            return new Vector3Int(x, y, z);
        }

        private static int GetIndex(int x, int y, int z, Vector3Int size)
            => x + size.x * (y + size.y * z);
        private static int GetIndex(Vector3Int idx, Vector3Int size)
            => GetIndex(idx.x, idx.y, idx.z, size);

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

        public float this[int idx]
        {
            get => field[idx];
            set => field[idx] = value;
        }
    }
}