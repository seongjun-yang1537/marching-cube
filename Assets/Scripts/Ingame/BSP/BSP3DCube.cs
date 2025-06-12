
using System;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public enum BSP3DCubeFace
    {
        FRONT,
        BACK,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
    }

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

        public List<BSP3DPlane> Faces => Enum
            .GetValues(typeof(BSP3DCubeFace))
            .Cast<BSP3DCubeFace>()
            .Select(face => GetFace(face))
            .ToList();

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

        public bool Contains(Vector3Int point)
        {
            return point.x >= topLeft.x && point.x < bottomRight.x &&
                   point.y >= topLeft.y && point.y < bottomRight.y &&
                   point.z >= topLeft.z && point.z < bottomRight.z;
        }

        public bool Contains(Vector3 point)
        {
            return point.x >= topLeft.x && point.x < bottomRight.x &&
                   point.y >= topLeft.y && point.y < bottomRight.y &&
                   point.z >= topLeft.z && point.z < bottomRight.z;
        }

        public BSP3DPlane GetFace(BSP3DCubeFace face)
        {
            return face switch
            {
                BSP3DCubeFace.LEFT => new BSP3DPlane(
                    new Vector3Int(topLeft.x, topLeft.y, topLeft.z),
                    new Vector3Int(topLeft.x, bottomRight.y, bottomRight.z),
                    Vector3Int.left
                ),

                BSP3DCubeFace.RIGHT => new BSP3DPlane(
                    new Vector3Int(bottomRight.x, topLeft.y, topLeft.z),
                    new Vector3Int(bottomRight.x, bottomRight.y, bottomRight.z),
                    Vector3Int.right
                ),

                BSP3DCubeFace.BOTTOM => new BSP3DPlane(
                    new Vector3Int(topLeft.x, topLeft.y, topLeft.z),
                    new Vector3Int(bottomRight.x, topLeft.y, bottomRight.z),
                    Vector3Int.down
                ),

                BSP3DCubeFace.TOP => new BSP3DPlane(
                    new Vector3Int(topLeft.x, bottomRight.y, topLeft.z),
                    new Vector3Int(bottomRight.x, bottomRight.y, bottomRight.z),
                    Vector3Int.up
                ),

                BSP3DCubeFace.FRONT => new BSP3DPlane(
                    new Vector3Int(topLeft.x, topLeft.y, topLeft.z),
                    new Vector3Int(bottomRight.x, bottomRight.y, topLeft.z),
                    Vector3Int.back
                ),

                BSP3DCubeFace.BACK => new BSP3DPlane(
                    new Vector3Int(topLeft.x, topLeft.y, bottomRight.z),
                    new Vector3Int(bottomRight.x, bottomRight.y, bottomRight.z),
                    Vector3Int.forward
                ),

                _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
            };

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
}