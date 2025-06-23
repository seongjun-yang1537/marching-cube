using System;
using System.Collections;
using Corelib.SUI;
using UnityEngine;

namespace MCube
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ScalarFieldModel : MonoBehaviour
    {
        public ScalarField scalarField;
        private Matrix4x4 trMat
        {
            get => Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        }

        public Material caveMaterial;

        [HideInInspector]
        public bool bVisibleScalarField = true;
        [HideInInspector]
        public bool bVisibleMarchingCubeGizmos = true;

        public void GenerateMarchingCube()
        {
            GenerateMarchingCubeMesh();
        }

        public IEnumerator GenerateMarchingCubeMesh()
        {
            Mesh mesh = new();
            yield return CreateMarchingCubeMesh(ret => mesh = ret);

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshCollider meshCollider = GetComponent<MeshCollider>();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;

            yield return null;
        }

        public IEnumerator CreateMarchingCubeMesh(Action<Mesh> callback)
        {
            Mesh mesh = new();
            yield return MarchingCubeMesher.Create(scalarField).Build(ret => mesh = ret);

            // MarchingCubeMesher.ApplyContourLineColor(scalarField, mesh);
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            meshRenderer.material = caveMaterial;

            callback(mesh);
        }

        public void OnDrawGizmosSelected()
        {
            DrawGizmosScalarField();
        }

        private void DrawGizmosScalarField()
        {
            if (!bVisibleScalarField || !scalarField)
                return;

            foreach (Vector3Int idx in scalarField.Indices)
            {
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
    }
}
