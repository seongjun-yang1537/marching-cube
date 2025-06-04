using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Ingame
{
    [Serializable]
    public class BSP3DCube
    {
        public Vector3Int topLeft, bottomRight;

        public Vector3Int size { get => bottomRight - topLeft; }
        public int lenX { get => bottomRight.x - topLeft.x; }
        public int lenY { get => bottomRight.y - topLeft.y; }
        public int lenZ { get => bottomRight.z - topLeft.z; }

        public int area { get => (bottomRight - topLeft).Area(); }
        public Vector3 center { get => (Vector3)(topLeft + bottomRight) / 2; }

        public BSP3DCube()
        {

        }
        public BSP3DCube(Vector3Int topLeft, Vector3Int bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;

            if (!IsValidate())
                throw new ArgumentException($"Invalid cube bounds: topLeft {topLeft} must be less than or equal to bottomRight {bottomRight}.");
        }

        private bool IsValidate() => topLeft.LessEqual(bottomRight);

        public bool Overlaps(BSP3DCube other)
        {
            if (bottomRight.x < other.topLeft.x || other.bottomRight.x < topLeft.x)
                return false;
            if (bottomRight.y < other.topLeft.y || other.bottomRight.y < topLeft.y)
                return false;
            if (bottomRight.z < other.topLeft.z || other.bottomRight.z < topLeft.z)
                return false;

            return true;
        }

        public bool IsAdjacent(BSP3DCube other) => GetAdjacentRegion(other) != null;

        public BSP3DCube GetAdjacentRegion(BSP3DCube other)
        {
            Vector3Int minA = Vector3Int.Min(this.topLeft, this.bottomRight);
            Vector3Int maxA = Vector3Int.Max(this.topLeft, this.bottomRight);
            Vector3Int minB = Vector3Int.Min(other.topLeft, other.bottomRight);
            Vector3Int maxB = Vector3Int.Max(other.topLeft, other.bottomRight);

            if (maxA.x == minB.x || maxB.x == minA.x)
            {
                int x = (maxA.x == minB.x) ? maxA.x : maxB.x;

                int yMin = Mathf.Max(minA.y, minB.y);
                int yMax = Mathf.Min(maxA.y, maxB.y);
                int zMin = Mathf.Max(minA.z, minB.z);
                int zMax = Mathf.Min(maxA.z, maxB.z);

                if (yMin < yMax && zMin < zMax)
                {
                    return new BSP3DCube(
                        new Vector3Int(x, yMin, zMin),
                        new Vector3Int(x, yMax, zMax)
                    );
                }
            }

            if (maxA.y == minB.y || maxB.y == minA.y)
            {
                int y = (maxA.y == minB.y) ? maxA.y : maxB.y;

                int xMin = Mathf.Max(minA.x, minB.x);
                int xMax = Mathf.Min(maxA.x, maxB.x);
                int zMin = Mathf.Max(minA.z, minB.z);
                int zMax = Mathf.Min(maxA.z, maxB.z);

                if (xMin < xMax && zMin < zMax)
                {
                    return new BSP3DCube(
                        new Vector3Int(xMin, y, zMin),
                        new Vector3Int(xMax, y, zMax)
                    );
                }
            }

            if (maxA.z == minB.z || maxB.z == minA.z)
            {
                int z = (maxA.z == minB.z) ? maxA.z : maxB.z;

                int xMin = Mathf.Max(minA.x, minB.x);
                int xMax = Mathf.Min(maxA.x, maxB.x);
                int yMin = Mathf.Max(minA.y, minB.y);
                int yMax = Mathf.Min(maxA.y, maxB.y);

                if (xMin < xMax && yMin < yMax)
                {
                    return new BSP3DCube(
                        new Vector3Int(xMin, yMin, z),
                        new Vector3Int(xMax, yMax, z)
                    );
                }
            }

            return null;
        }
    }

    [Serializable]
    public class BSP3DPlane : BSP3DCube
    {
        public Vector3Int size { get => bottomRight - topLeft; }
        public Vector3 center { get => (Vector3)(topLeft + bottomRight) / 2; }

        public BSP3DPlane() : base()
        {

        }
        public BSP3DPlane(Vector3Int topLeft, Vector3Int bottomRight) : base(topLeft, bottomRight)
        {
        }
    }
    [Serializable]
    public class BSP3DRoom : BSP3DCube
    {
        public int idx;

        public BSP3DRoom() : base()
        {

        }
        public BSP3DRoom(Vector3Int topLeft, Vector3Int bottomRight) : base(topLeft, bottomRight)
        {
        }
    }

    [Serializable]
    public class BSP3DTreeNode
    {
        public struct BSP3DRoomGenerationParmas
        {
            public Vector3Int? minSize;
            public Vector3Int? maxSize;
            public Vector3? minSizeRatio;
            public Vector3? maxSizeRatio;

            private MT19937 _rng;
            public MT19937 rng
            {
                get => _rng ??= MT19937.Create();
                set => _rng = value;
            }
        }

        [SerializeField]
        public List<BSP3DTreeNode> childs;
        public BSP3DCube cube;
        [SerializeField]
        public BSP3DPlane separator;
        [SerializeField]
        public BSP3DRoom room;

        public int depth;
        public bool isLeaf { get => childs.Count == 0; }

        public BSP3DTreeNode()
        {
            childs = new();
            separator = new();
        }
        public BSP3DTreeNode(Vector3Int topLeft, Vector3Int bottomRight) : this()
        {
            cube = new(topLeft, bottomRight);
        }

        public List<BSP3DTreeNode> ToList()
        {
            List<BSP3DTreeNode> list = new();
            Queue<BSP3DTreeNode> queue = new();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                BSP3DTreeNode top = queue.Dequeue();
                list.Add(top);
                foreach (var child in top.childs)
                    queue.Enqueue(child);
            }
            return list;
        }

        public void GenerateRoom(in BSP3DRoomGenerationParmas param)
        {
            Vector3Int minSize = Vector3Int.one;
            if (param.minSize != null)
                minSize = Vector3Int.Max(minSize, param.minSize.Value);
            if (param.minSizeRatio != null)
                minSize = Vector3Int.Max(
                    minSize,
                    Vector3Int.one.Max(Vector3.Scale(cube.size, param.minSizeRatio.Value).CeilToInt())
                );

            Vector3Int maxSize = cube.size;
            if (param.maxSize != null)
                maxSize = Vector3Int.Min(maxSize, param.maxSize.Value);
            if (param.maxSizeRatio != null)
                maxSize = Vector3Int.Min(
                    maxSize,
                    Vector3Int.one.Max(Vector3.Scale(cube.size, param.maxSizeRatio.Value).CeilToInt())
                );

            Vector3Int roomSize = minSize.RandomRange(maxSize, param.rng);

            Vector3Int remainDelta = cube.size - roomSize;
            Vector3Int moveDelta = Vector3Int.zero.RandomRange(remainDelta, param.rng);

            Vector3Int topLeft = cube.topLeft + moveDelta;
            Vector3Int bottomRight = topLeft + roomSize;
            room = new BSP3DRoom(topLeft, bottomRight);
        }

        public void GenerateRoom(MT19937 rng = null)
        {
            BSP3DRoomGenerationParmas param = new()
            {
                rng = rng,
                minSizeRatio = new Vector3(0.6f, 0.6f, 0.6f),
                maxSizeRatio = new Vector3(0.9f, 0.9f, 0.9f),
            };
            GenerateRoom(param);
        }

        public void GenerateJunctionRoom(MT19937 rng = null)
        {
            BSP3DRoomGenerationParmas param = new()
            {
                rng = rng,
                minSize = Vector3Int.one * 3,
                maxSize = Vector3Int.one * 3,
            };
            GenerateRoom(param);
        }
    }
}