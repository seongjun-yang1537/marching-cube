using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
