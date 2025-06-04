using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Corelib.SUI;
using Unity.VisualScripting;
using UnityEngine;

namespace MCube
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ScalarFieldModel : MonoBehaviour
    {
        public ScalarField scalarField;
        private Matrix4x4 trMat
        {
            get => Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        }

        public Material caveMaterial;

        List<Vector3> marchingCubeTriangles;
        private Material _material;

        [HideInInspector]
        public bool bVisibleScalarField = true;
        [HideInInspector]
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
            Mesh mesh = MarchingCubeMesher.Create(scalarField).Build();
            MarchingCubeMesher.ApplyContourLineColor(scalarField, mesh);

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            meshRenderer.material = caveMaterial;

            return mesh;
        }

        public void OnDrawGizmosSelected()
        {
            OnDrawGizmosMarchingCubes();
            DrawGizmosScalarField();
        }

        private void DrawGizmosScalarField()
        {
            if (!bVisibleScalarField || !scalarField)
                return;

            foreach (Vector3Int idx in scalarField.Indices)
            {
                // SGizmos.Scope(
                //     SGizmos.Sphere(idx, 0.25f)
                // )
                // .Color(new Color(1f, 1f, 1f, 1.0f - scalarField[idx]))
                // .Matrix(transform.localToWorldMatrix)
                // .Render();

                if (Mathf.Approximately(scalarField[idx], 0.0f))
                {
                    SGizmos.Scope(
                        SGizmos.Sphere(idx, 0.25f)
                    )
                    .Color(new Color(1f, 1f, 1f, 1.0f))
                    .Matrix(transform.localToWorldMatrix)
                    .Render();
                }
            }
        }

        private void OnDrawGizmosMarchingCubes()
        {
            if (!bVisibleMarchingCubeGizmos || !scalarField)
                return;
            if (marchingCubeTriangles == null)
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
    }
}
