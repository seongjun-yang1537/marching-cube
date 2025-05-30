using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace MCube
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ScalarFieldModel : MonoBehaviour
    {
        public ScalarField scalarField;
        [Range(0f, 1f)]
        public float threshold;

        private Matrix4x4 trMat
        {
            get => Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        }

        List<Vector3> marchingCubeTriangles;
        private Material _material;

        public bool bVisibleScalarField = true;
        public bool bVisibleMarchingCubeGizmos = true;

        private Material CreateMaterial()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            Material _material = new Material(shader);
            _material.hideFlags = HideFlags.HideAndDontSave;
            _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _material.SetInt("_ZWrite", 0);
            return _material;
        }

        public void GenerateMarchingCube()
        {
            marchingCubeTriangles = new();

            Vector3Int size = scalarField.size - Vector3Int.one;
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);

                        List<bool> mask = GetMarchingCubeMask(new Vector3Int(x, y, z));
                        List<Vector3> triangles = MarchingCube.GetTriangle(mask)
                            .Select(delta => pos + delta)
                            .ToList();
                        marchingCubeTriangles.AddRange(triangles);
                    }

            GenerateMarchingCubeMesh();
        }

        public void GenerateMarchingCubeMesh()
        {
            Mesh mesh = CreateMarchingCubeMesh();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        public Mesh CreateMarchingCubeMesh()
        {
            Mesh mesh = new Mesh();

            Dictionary<Vector2Int, int> vertexIdx = new();
            Vector3Int size0 = scalarField.size - Vector3Int.one;
            for (int x = 0; x < size0.x; x++)
                for (int y = 0; y < size0.y; y++)
                    for (int z = 0; z < size0.z; z++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        List<bool> maskList = GetMarchingCubeMask(pos);
                        int mask = 0;
                        for (int i = 0; i < maskList.Count; i++)
                        {
                            if (maskList[i])
                            {
                                mask |= 1 << i;
                            }
                        }

                        List<Vector3Int> deltaPoses = MarchingCubeTable.Delta
                        .Select(delta => pos + delta)
                        .ToList();

                        int edgeMask = MarchingCubeTable.EdgeMasks[mask];
                        List<int> usedEdge = new();
                        for (int i = 0; i < MarchingCubeTable.EDGE_COUNT; i++)
                        {
                            if ((edgeMask & (1 << i)) > 0)
                            {
                                usedEdge.Add(i);
                            }
                        }

                        int[,] EdgeVertexIndices = MarchingCubeTable.EdgeVertexIndices;
                        foreach (int edgeIdx in usedEdge)
                        {
                            Vector3Int fromPos = deltaPoses[EdgeVertexIndices[edgeIdx, 0]];
                            Vector3Int toPos = deltaPoses[EdgeVertexIndices[edgeIdx, 1]];

                            int fromIdx = scalarField.GetIndex(fromPos);
                            int toIdx = scalarField.GetIndex(toPos);
                            Vector2Int vecKey = new Vector2Int(fromIdx, toIdx);

                            if (!vertexIdx.ContainsKey(vecKey))
                            {
                                vertexIdx.Add(vecKey, vertexIdx.Count);
                            }
                        }
                    }

            List<Vector3> vertices = new();
            foreach (var key in vertexIdx.Keys)
            {
                int from = key.x;
                int to = key.y;
                Vector3 fromPos = scalarField.SpreadIndex(from);
                Vector3 toPos = scalarField.SpreadIndex(to);
                vertices.Add((fromPos + toPos) / 2);
            }

            List<int> triangles = new();
            for (int x = 0; x < size0.x; x++)
                for (int y = 0; y < size0.y; y++)
                    for (int z = 0; z < size0.z; z++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        List<bool> maskList = GetMarchingCubeMask(pos);
                        int mask = 0;
                        for (int i = 0; i < maskList.Count; i++)
                        {
                            if (maskList[i])
                            {
                                mask |= 1 << i;
                            }
                        }

                        List<Vector3Int> deltaPoses = MarchingCubeTable.Delta
                        .Select(delta => pos + delta)
                        .ToList();

                        int[] triangleTable = MarchingCubeTable.TriangleTable[mask];
                        for (int i = 0; i <= triangleTable.Length - 3; i += 3)
                        {
                            int a = triangleTable[i];
                            int b = triangleTable[i + 1];
                            int c = triangleTable[i + 2];

                            int au = MarchingCubeTable.EdgeVertexIndices[a, 0];
                            int av = MarchingCubeTable.EdgeVertexIndices[a, 1];

                            int bu = MarchingCubeTable.EdgeVertexIndices[b, 0];
                            int bv = MarchingCubeTable.EdgeVertexIndices[b, 1];

                            int cu = MarchingCubeTable.EdgeVertexIndices[c, 0];
                            int cv = MarchingCubeTable.EdgeVertexIndices[c, 1];



                            triangles.Add(vertexIdx[
                                new Vector2Int(scalarField.GetIndex(deltaPoses[au]), scalarField.GetIndex(deltaPoses[av]))]);
                            triangles.Add(vertexIdx[
                                new Vector2Int(scalarField.GetIndex(deltaPoses[bu]), scalarField.GetIndex(deltaPoses[bv]))]);
                            triangles.Add(vertexIdx[
                                new Vector2Int(scalarField.GetIndex(deltaPoses[cu]), scalarField.GetIndex(deltaPoses[cv]))]);
                        }
                    }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            float minY = 0.0f; // 최소 Y값 (예시)
            float maxY = scalarField.size.y; // 최대 Y값 (예시)
            float hueScrollSpeed = 0.1f; // 시간에 따라 Hue가 변하게 하려면 (선택 사항)

            // 메시에 할당할 색상 배열 생성
            Color[] colors = new Color[mesh.vertices.Length];

            // --- 여기에 육지 등고선 색상 팔레트 정의 ---
            Color[] contourPalette = new Color[] {
                // 육지 시작 (낮은 곳 -> 높은 곳 순서)
                new Color(0.0f, 0.5f, 0.0f),   // 짙은 녹색 (아주 낮은 땅)
                new Color(0.2f, 0.6f, 0.1f),   // 녹색
                new Color(0.6f, 0.8f, 0.2f),   // 연한 녹색 / 황록색 (평야)
                new Color(0.8f, 0.7f, 0.3f),   // 노란색 / 모래색 (낮은 구릉)
                new Color(0.6f, 0.4f, 0.2f),   // 연한 갈색 (구릉/산기슭)
                new Color(0.45f, 0.3f, 0.15f),  // 갈색 (산)
                new Color(0.3f, 0.25f, 0.2f),  // 짙은 갈색 / 회갈색 (높은 산)
                new Color(0.5f, 0.5f, 0.5f),   // 회색 (더 높은 산)
                Color.white                    // 흰색 (최고봉, 눈)
                // 필요하다면 이 앞에 바다색을 추가할 수도 있습니다.
                // 예: new Color(0.0f, 0.0f, 0.5f), // 짙은 파랑 (깊은 물)
            };
            // --- 팔레트 정의 끝 ---

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                float currentY = mesh.vertices[i].y; // 현재 로컬 Y좌표 사용 중
                float normalizedY = Mathf.InverseLerp(minY, maxY, currentY);

                // --- Hue 계산 부분을 팔레트 인덱싱으로 변경 ---
                // int numberOfBanks = 10; // 팔레트 방식에서는 팔레트 배열의 크기가 밴드 수를 결정
                // float bandHue = Mathf.Floor(normalizedY * numberOfBanks) / (float)numberOfBanks;
                // float hue = bandHue;
                // float saturation = 1.0f;
                // float value = 1.0f;
                // colors[i] = Color.HSVToRGB(hue, saturation, value);
                // --- 위 Hue 계산 로직 대신 아래 팔레트 조회 로직 사용 ---

                // normalizedY 값 (0~1)을 사용하여 contourPalette 배열의 인덱스를 계산
                // normalizedY가 1일 때 마지막 인덱스(Length-1)가 되도록 계산
                int bandIndex = Mathf.FloorToInt(normalizedY * (contourPalette.Length - 0.0001f));
                // Clamp를 사용하여 bandIndex가 배열 범위를 벗어나지 않도록 보정
                bandIndex = Mathf.Clamp(bandIndex, 0, contourPalette.Length - 1);

                colors[i] = contourPalette[bandIndex]; // 팔레트에서 직접 색상 선택
            }
            mesh.colors = colors; // 계산된 색상 배열을 메시에 할당

            Material newMaterial = new Material(Shader.Find("Custom/UnlitVertexColor"));
            meshRenderer.material = newMaterial;

            return mesh;
        }

        private List<bool> GetMarchingCubeMask(Vector3Int pos)
        {
            List<bool> mask = new();

            foreach (var delta in MarchingCubeTable.Delta)
            {
                mask.Add(scalarField[pos + delta] >= threshold);
            }

            return mask;
        }

        public void OnDrawGizmosSelected()
        {
            OnDrawGizmosMarchingCubes();
            OnDrawGizmosScalarField();
        }

        private void OnDrawGizmosMarchingCubes()
        {
            if (!bVisibleMarchingCubeGizmos || !scalarField)
                return;

            if (!_material) _material = CreateMaterial();
            _material.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.TRIANGLES);
            GL.Color(new Color(1f, 0f, 0f, 0.5f));

            for (int i = 0; i < marchingCubeTriangles.Count; i += 3)
            {
                GL.Vertex(marchingCubeTriangles[i]);
                GL.Vertex(marchingCubeTriangles[i + 1]);
                GL.Vertex(marchingCubeTriangles[i + 2]);
            }

            GL.End();
            GL.PopMatrix();
        }

        private void OnDrawGizmosScalarField()
        {
            if (!bVisibleScalarField || !scalarField)
                return;

            using (GizmoContext.New().Matrix(trMat))
            {
                Vector3Int size = scalarField.size;
                Gizmos.DrawWireCube(Vector3.zero + (size - Vector3.one) / 2, size - Vector3.one);

                foreach (Vector3Int i in scalarField.Index())
                {
                    float weight = scalarField[i];
                    if (weight < threshold)
                    {
                        continue;
                    }
                    float color = 0.1f + 0.9f * weight;
                    using (GizmoContext.New()
                        .Color(new Color(color, color, color, 0.5f)))
                    {
                        Gizmos.DrawSphere(i, 0.1f);
                    }
                }
            }
        }
    }
}
