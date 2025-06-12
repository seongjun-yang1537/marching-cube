using System;
using System.Collections.Generic;
using Corelib.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace MCube
{
    [CreateAssetMenu(fileName = "New Scalar Field", menuName = "ScriptableObject/Scalar Field")]
    public class ScalarField : SerializedScriptableObject
    {
        private const long LIMIT_SIZE = 3_000_000;

        public Vector3Int size;
        public int count { get => size.x * size.y * size.z; }
        public float[] field;
        public float threshold;

        public IEnumerable<Vector3Int> Indices { get => CIterator.GetArray3D(size); }

        public ScalarField(ScalarField source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "복사할 원본 ScalarField 객체는 null일 수 없습니다.");
            }

            this.size = source.size;
            this.threshold = source.threshold;
            if (this.count > 0)
            {
                this.field = new float[this.count];

                if (source.field != null)
                {
                    int elementsToCopy = Math.Min(source.field.Length, this.count);
                    Array.Copy(source.field, 0, this.field, 0, elementsToCopy);
                }
            }
            else
            {
                this.field = new float[0];
            }
        }

        public void All(float value)
        {
            for (int i = 0; i < field.Length; i++)
                field[i] = value;
        }

        public void ThreshodlMap(Func<Vector3Int, float> mapping)
        {
            for (int i = 0; i < field.Length; i++)
                if (field[i] >= threshold) field[i] = mapping(SpreadIndex(i));
        }

        public void Inverse()
        {
            for (int i = 0; i < field.Length; i++)
                field[i] = 1.0f - field[i];
        }

        public bool InRange(Vector3Int vec)
        {
            if (vec.x < 0 || size.x <= vec.x) return false;
            if (vec.y < 0 || size.y <= vec.y) return false;
            if (vec.z < 0 || size.z <= vec.z) return false;
            return true;
        }

        public List<List<Vector3Int>> GetComponents(Func<Vector3Int, bool> func)
        {
            List<List<Vector3Int>> components = new();
            List<bool> visit = new List<bool>().Resize(count);

            List<Vector3Int> bfs(Vector3Int startPos)
            {
                Queue<Vector3Int> queue = new();
                queue.Enqueue(startPos);
                List<Vector3Int> component = new();

                while (queue.Count > 0)
                {
                    Vector3Int top = queue.Dequeue();
                    component.Add(top);

                    foreach (Vector3Int delta in ExVector3Int.DIR6)
                    {
                        Vector3Int to = top + delta;
                        int toIdx = GetIndex(to);
                        if (!InRange(to) || !func(to) || visit[toIdx])
                            continue;
                        visit[toIdx] = true;
                        queue.Enqueue(to);
                    }
                }

                return component;
            }

            foreach (Vector3Int pos in Indices)
            {
                int idx = GetIndex(pos);
                if (visit[idx] || !func(pos))
                    continue;
                components.Add(bfs(pos));
            }

            return components;
        }

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

        public void AddPadding(int paddingWidth, float paddingValue)
        {
            if (paddingWidth < 0)
            {
                Debug.LogError("Padding width cannot be negative.");
                return;
            }
            if (paddingWidth == 0)
            {
                Debug.Log("Padding width is zero. No changes made to the field.");
                return;
            }

            Vector3Int originalSize = this.size;
            float[] originalField = this.field;

            Vector3Int newPaddedSize = new Vector3Int(
                originalSize.x + 2 * paddingWidth,
                originalSize.y + 2 * paddingWidth,
                originalSize.z + 2 * paddingWidth
            );

            if (IsLimitSize(newPaddedSize))
            {
                long newTotalCount = (long)newPaddedSize.x * newPaddedSize.y * newPaddedSize.z;
                Debug.LogWarning($"Padded size {newPaddedSize} (total elements: {newTotalCount}) exceeds limit ({LIMIT_SIZE}). Padding aborted.");
                return;
            }

            long newCount = (long)newPaddedSize.x * newPaddedSize.y * newPaddedSize.z;
            float[] newField = new float[newCount];

            if (newCount > 0)
            {
                for (int i = 0; i < newCount; i++)
                {
                    newField[i] = paddingValue;
                }
            }

            if (originalField != null && originalSize.x > 0 && originalSize.y > 0 && originalSize.z > 0)
            {
                foreach (Vector3Int originalCoord in CIterator.GetArray3D(originalSize)) // CIterator.GetArray3D가 있다고 가정
                {
                    int originalIndex = GetIndex(originalCoord.x, originalCoord.y, originalCoord.z, originalSize);

                    Vector3Int newCoordInPaddedField = new Vector3Int(
                        originalCoord.x + paddingWidth,
                        originalCoord.y + paddingWidth,
                        originalCoord.z + paddingWidth
                    );

                    int newPaddedIndex = GetIndex(newCoordInPaddedField.x, newCoordInPaddedField.y, newCoordInPaddedField.z, newPaddedSize);

                    if (originalIndex >= 0 && originalIndex < originalField.Length &&
                        newPaddedIndex >= 0 && newPaddedIndex < newField.Length)
                    {
                        newField[newPaddedIndex] = originalField[originalIndex];
                    }
                }
            }

            this.size = newPaddedSize;
            this.field = newField;
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

        public bool IsSolid(Vector3Int idx)
            => this[idx] >= threshold;

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