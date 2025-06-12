using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace MCube
{
    public class MarchingCubeMesher
    {
        private readonly ScalarField field;
        private float isoLevel = 0.5f;
        private bool interpolate = true;

        private MarchingCubeMesher(ScalarField field)
        {
            this.field = field;
        }

        public static MarchingCubeMesher Create(ScalarField field)
        {
            return new MarchingCubeMesher(field);
        }

        public static void ApplyContourLineColor(ScalarField field, Mesh mesh)
        {
            float minY = 0.0f;
            float maxY = field.size.y;
            float hueScrollSpeed = 0.1f;

            Color[] colors = new Color[mesh.vertices.Length];

            Color[] contourPalette = new Color[] {
                new Color(0.0f, 0.5f, 0.0f),
                new Color(0.2f, 0.6f, 0.1f),
                new Color(0.6f, 0.8f, 0.2f),
                new Color(0.8f, 0.7f, 0.3f),
                new Color(0.6f, 0.4f, 0.2f),
                new Color(0.45f, 0.3f, 0.15f),
                new Color(0.3f, 0.25f, 0.2f),
                new Color(0.5f, 0.5f, 0.5f),
                Color.white
            };

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                float currentY = mesh.vertices[i].y; // 현재 로컬 Y좌표 사용 중
                float normalizedY = Mathf.InverseLerp(minY, maxY, currentY);

                int bandIndex = Mathf.FloorToInt(normalizedY * (contourPalette.Length - 0.0001f));
                bandIndex = Mathf.Clamp(bandIndex, 0, contourPalette.Length - 1);

                colors[i] = contourPalette[bandIndex];
            }
            mesh.colors = colors;
        }

        public MarchingCubeMesher SetIsoLevel(float iso)
        {
            this.isoLevel = iso;
            return this;
        }

        public MarchingCubeMesher EnableInterpolation(bool enabled)
        {
            this.interpolate = enabled;
            return this;
        }

        private int GetCubeMask(Vector3Int pos)
        {
            int mask = 0;
            var delta = MarchingCubeTable.Delta;
            for (int i = 0; i < MarchingCubeTable.VERTEX_COUNT; i++)
            {
                if (field[pos + delta[i]] >= field.threshold)
                {
                    mask |= 1 << i;
                }
            }
            return mask;
        }

        private IEnumerator CreateVertexIndices(Action<Dictionary<Vector2Int, int>> callback)
        {
            const int MAX_BATCH = 100;
            int batch = 0;

            Dictionary<Vector2Int, int> vertexIdx = new();
            foreach (var pos in CIterator.GetArray3D(field.size - Vector3Int.one))
            {
                int mask = GetCubeMask(pos);
                List<Vector3Int> deltaPositions = MarchingCube.GetDelatPositions(pos);
                foreach (var (u, v) in MarchingCube.GetEdgeIterator(mask))
                {
                    if (++batch == MAX_BATCH)
                    {
                        yield return null;
                        batch = 0;
                    }

                    (Vector3Int fromPos, Vector3Int toPos) = (deltaPositions[u], deltaPositions[v]);
                    (int fromIdx, int toIdx) = (field.GetIndex(fromPos), field.GetIndex(toPos));

                    Vector2Int idxEdge = new Vector2Int(fromIdx, toIdx);
                    if (vertexIdx.ContainsKey(idxEdge))
                        continue;
                    vertexIdx.Add(idxEdge, vertexIdx.Count);
                }
            }
            callback(vertexIdx);
        }

        private List<Vector3> CreateVertices(Dictionary<Vector2Int, int> vertexIndices)
            => vertexIndices.Keys
            .Select(edge =>
            {
                var (u, v) = edge;
                return (Vector3)(field.SpreadIndex(u) + field.SpreadIndex(v)) / 2;
            }).ToList();

        private IEnumerator CreateTriangles(Dictionary<Vector2Int, int> vertexIndices, Action<List<int>> callback)
        {
            const int MAX_BATCH = 100;
            int batch = 0;

            List<int> triangles = new();
            foreach (var pos in CIterator.GetArray3D(field.size - Vector3Int.one))
            {
                int mask = GetCubeMask(pos);
                List<Vector3Int> deltaPositions = MarchingCube.GetDelatPositions(pos);
                void AddEdge(int idx)
                {
                    int u = MarchingCubeTable.EdgeVertexIndices[idx, 0];
                    int v = MarchingCubeTable.EdgeVertexIndices[idx, 1];
                    triangles.Add(vertexIndices[new Vector2Int(
                        field.GetIndex(deltaPositions[u]),
                        field.GetIndex(deltaPositions[v])
                    )]);
                }
                void AddTriangle(int a, int b, int c)
                {
                    AddEdge(a);
                    AddEdge(b);
                    AddEdge(c);
                }

                int[] triangleTable = MarchingCubeTable.TriangleTable[mask];
                for (int i = 0; i <= triangleTable.Length - 3; i += 3)
                {
                    AddTriangle(triangleTable[i], triangleTable[i + 1], triangleTable[i + 2]);

                    if (++batch == MAX_BATCH)
                    {
                        yield return null;
                        batch = 0;
                    }
                }
                callback(triangles);
            }
        }

        public IEnumerator Build(Action<Mesh> callback)
        {
            Mesh mesh = new Mesh();

            Dictionary<Vector2Int, int> vertexIndices = new();
            yield return CreateVertexIndices(ret => vertexIndices = ret);

            mesh.vertices = CreateVertices(vertexIndices).ToArray();

            List<int> triangles = new();
            yield return CreateTriangles(vertexIndices, ret => triangles = ret);
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            callback(mesh);
        }
    }
}