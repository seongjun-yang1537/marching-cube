using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Corelib.Utils;
using Sirenix.Utilities;
using UnityEngine;

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

        private Dictionary<Vector2Int, int> CreateVertexIndices()
        {
            Dictionary<Vector2Int, int> vertexIdx = new();
            CIterator.GetArray3D(field.size - Vector3Int.one)
            .ForEach(pos =>
            {
                int mask = GetCubeMask(pos);
                List<Vector3Int> deltaPositions = MarchingCube.GetDelatPositions(pos);
                foreach (var (u, v) in MarchingCube.GetEdgeIterator(mask))
                {
                    (Vector3Int fromPos, Vector3Int toPos) = (deltaPositions[u], deltaPositions[v]);
                    (int fromIdx, int toIdx) = (field.GetIndex(fromPos), field.GetIndex(toPos));

                    Vector2Int idxEdge = new Vector2Int(fromIdx, toIdx);
                    if (vertexIdx.ContainsKey(idxEdge))
                        continue;
                    vertexIdx.Add(idxEdge, vertexIdx.Count);
                }
            });
            return vertexIdx;
        }

        private List<Vector3> CreateVertices(Dictionary<Vector2Int, int> vertexIndices)
            => vertexIndices.Keys
            .Select(edge =>
            {
                var (u, v) = edge;
                return (Vector3)(field.SpreadIndex(u) + field.SpreadIndex(v)) / 2;
            }).ToList();

        private List<int> CreateTriangles(Dictionary<Vector2Int, int> vertexIndices)
        {
            List<int> triangles = new();
            CIterator.GetArray3D(field.size - Vector3Int.one)
            .ForEach(pos =>
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
                }
            });
            return triangles;
        }

        public Mesh Build()
        {
            Mesh mesh = new Mesh();

            Dictionary<Vector2Int, int> vertexIndices = CreateVertexIndices();

            mesh.vertices = CreateVertices(vertexIndices).ToArray();
            mesh.triangles = CreateTriangles(vertexIndices).ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }
    }
}