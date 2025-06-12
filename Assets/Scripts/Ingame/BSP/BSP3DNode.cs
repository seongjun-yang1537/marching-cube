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
    public enum PlaneFixedAxis
    {
        X, Y, Z
    }

    [Serializable]
    public class BSP3DPlane : BSP3DCube
    {
        public Vector3Int normal;

        public BSP3DPlane() : base()
        {

        }
        public BSP3DPlane(Vector3Int topLeft, Vector3Int bottomRight) : base(topLeft, bottomRight)
        {
        }
        public BSP3DPlane(Vector3Int topLeft, Vector3Int bottomRight, Vector3Int normal) : base(topLeft, bottomRight)
        {
            this.normal = normal;
        }

        public PlaneFixedAxis GetFixedAxis()
        {
            if (topLeft.x == bottomRight.x) return PlaneFixedAxis.X;
            if (topLeft.y == bottomRight.y) return PlaneFixedAxis.Y;
            if (topLeft.z == bottomRight.z) return PlaneFixedAxis.Z;
            throw new InvalidOperationException("Plane is not flat on any axis.");
        }

        public Vector2Int Project2D(Vector3Int pos)
        {
            return GetFixedAxis() switch
            {
                PlaneFixedAxis.X => new Vector2Int(pos.y - topLeft.y, pos.z - topLeft.z),
                PlaneFixedAxis.Y => new Vector2Int(pos.x - topLeft.x, pos.z - topLeft.z),
                PlaneFixedAxis.Z => new Vector2Int(pos.x - topLeft.x, pos.y - topLeft.y),
                _ => throw new InvalidOperationException()
            };
        }

        public Vector3Int Unproject2D(Vector2Int pos2D)
        {
            return GetFixedAxis() switch
            {
                PlaneFixedAxis.X => new Vector3Int(topLeft.x, topLeft.y + pos2D.x, topLeft.z + pos2D.y),
                PlaneFixedAxis.Y => new Vector3Int(topLeft.x + pos2D.x, topLeft.y, topLeft.z + pos2D.y),
                PlaneFixedAxis.Z => new Vector3Int(topLeft.x + pos2D.x, topLeft.y + pos2D.y, topLeft.z),
                _ => throw new InvalidOperationException()
            };
        }

        public Vector3Int Project3D(Vector3Int pos)
        {
            return GetFixedAxis() switch
            {
                PlaneFixedAxis.X => new Vector3Int(topLeft.x, topLeft.y + pos.x, topLeft.z + pos.y),
                PlaneFixedAxis.Y => new Vector3Int(topLeft.x + pos.x, topLeft.y, topLeft.z + pos.y),
                PlaneFixedAxis.Z => new Vector3Int(topLeft.x + pos.x, topLeft.y + pos.y, topLeft.z),
                _ => throw new InvalidOperationException()
            };
        }

        public Vector2Int GetSize2D()
        {
            return GetFixedAxis() switch
            {
                PlaneFixedAxis.X => new Vector2Int(bottomRight.y - topLeft.y, bottomRight.z - topLeft.z),
                PlaneFixedAxis.Y => new Vector2Int(bottomRight.x - topLeft.x, bottomRight.z - topLeft.z),
                PlaneFixedAxis.Z => new Vector2Int(bottomRight.x - topLeft.x, bottomRight.y - topLeft.y),
                _ => throw new InvalidOperationException()
            };
        }

        public bool InRange2D(Vector2Int pos)
        {
            Vector2Int size = GetSize2D();
            return pos.GreaterEqual(Vector2Int.zero) && pos.Less(size);
        }

        public bool InRange3D(Vector3Int pos)
        {
            switch (GetFixedAxis())
            {
                case PlaneFixedAxis.X:
                    return pos.x == topLeft.x &&
                           pos.y >= topLeft.y && pos.y < bottomRight.y &&
                           pos.z >= topLeft.z && pos.z < bottomRight.z;

                case PlaneFixedAxis.Y:
                    return pos.y == topLeft.y &&
                           pos.x >= topLeft.x && pos.x < bottomRight.x &&
                           pos.z >= topLeft.z && pos.z < bottomRight.z;

                case PlaneFixedAxis.Z:
                    return pos.z == topLeft.z &&
                           pos.x >= topLeft.x && pos.x < bottomRight.x &&
                           pos.y >= topLeft.y && pos.y < bottomRight.y;

                default:
                    throw new InvalidOperationException("Invalid plane configuration.");
            }
        }

        public float DistanceToPoint(Vector3 point)
        {
            PlaneFixedAxis axis = GetFixedAxis();

            return axis switch
            {
                PlaneFixedAxis.X => Mathf.Abs(point.x - topLeft.x),
                PlaneFixedAxis.Y => Mathf.Abs(point.y - topLeft.y),
                PlaneFixedAxis.Z => Mathf.Abs(point.z - topLeft.z),
                _ => throw new InvalidOperationException()
            };
        }

        public float DistanceToPoint(Vector3Int point) => DistanceToPoint((Vector3)point);
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
            => GenerateRoom(rng, Vector3.one, Vector3.one);

        public void GenerateRoom(MT19937 rng, Vector3 minSizeRatio, Vector3 maxSizeRatio)
        {
            BSP3DRoomGenerationParmas param = new()
            {
                rng = rng,
                minSizeRatio = minSizeRatio,
                maxSizeRatio = maxSizeRatio,
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