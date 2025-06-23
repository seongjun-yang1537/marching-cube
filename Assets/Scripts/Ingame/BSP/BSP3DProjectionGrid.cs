using System;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    [Serializable]
    public class BSP3DProjectionGrid
    {
        private Vector3Int NONE = -Vector3Int.one;

        public Vector2Int size;
        [SerializeField] public BSP3DPlane plane;
        [SerializeField] private Vector3Int[] grid;

        public int validCount { get => validIndicies.Count; }
        public List<Vector2Int> validIndicies;
        public List<Vector3Int> validPoses;

        public BSP3DProjectionGrid(Vector3Int center, BSP3DPlane plane, List<Vector3Int> poses)
        {
            this.plane = plane;
            InitializeGrid(center, poses);
            InitializePrecompute();
        }

        private void InitializeGrid(Vector3Int center, List<Vector3Int> poses)
        {
            size = plane.GetSize2D();
            grid = new Vector3Int[size.x * size.y];
            Array.Fill(grid, NONE);

            foreach (var pos in poses)
            {
                Vector2Int projected2D = plane.Project2D(pos);
                if (!plane.InRange2D(projected2D)) continue;
                Vector3 uv = pos - center;
                Vector3 wv = -plane.normal;
                if (Mathf.Approximately(Vector3.Cross(uv, wv).sqrMagnitude, 0f) || Vector3.Dot(uv, wv) > 0)
                    continue;

                int idx = GetIndex(projected2D);
                var prev = grid[idx];

                if (prev == NONE ||
                    (pos - center).sqrMagnitude > (prev - center).sqrMagnitude)
                {
                    grid[idx] = pos;
                }
            }
        }

        private void InitializePrecompute()
        {
            validIndicies = grid.Select((value, idx) => new { value = value, idx = idx })
            .Where(data => data.value != -Vector3Int.one)
            .Select(data => SpreadIndex(data.idx))
            .ToList();
            validPoses = validIndicies.Select(idx => this[idx]).ToList();
        }

        private int GetIndex(Vector2Int vec) => vec.x + vec.y * size.x;
        private Vector2Int SpreadIndex(int idx)
        {
            int x = idx % size.x;
            int y = idx / size.x;
            return new Vector2Int(x, y);
        }

        public bool RaycastLandscape(Vector2Int idx, out Vector3 ret, Matrix4x4 modelMatrix)
        {
            ret = NONE;

            if (this[idx] == NONE)
                return false;

            Vector3 origin = modelMatrix.MultiplyPoint(this[idx]);
            Debug.Log(origin);
            Vector3 normal = modelMatrix.MultiplyPoint(plane.normal);
            Ray ray = new Ray(origin, normal);

            if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << LayerMask.NameToLayer("Landscape")))
                return false;

            ret = hit.point;
            return true;
        }

        public List<Vector2Int> GetValidIndicies()
            => CIterator.GetArray2D(size).Where(idx => this[idx] != -Vector3Int.one).ToList();

        public Vector3Int this[int x, int y]
        {
            get => grid[x + y * size.x];
            set => grid[x + y * size.x] = value;
        }

        public Vector3Int this[Vector2Int idx]
        {
            get => this[idx.x, idx.y];
            set => this[idx.x, idx.y] = value;
        }
    }

}