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

        private MeshFilter _meshFilter;
        private MeshFilter MeshFilter { get => _meshFilter ??= GetComponent<MeshFilter>(); }


        private MeshCollider _meshCollider;
        private MeshCollider MeshCollider { get => _meshCollider ??= GetComponent<MeshCollider>(); }

        private MeshRenderer _meshRenderer;
        private MeshRenderer MeshRenderer { get => _meshRenderer ??= GetComponent<MeshRenderer>(); }

        public void GenerateMarchingCube()
        {
            GenerateMarchingCubeMesh();
        }

        public IEnumerator GenerateMarchingCubeMesh()
        {
            Mesh mesh = new();
            yield return CreateMarchingCubeMesh(ret => mesh = ret);

            MeshFilter.mesh = mesh;
            MeshCollider.sharedMesh = mesh;

            yield return null;
        }

        public IEnumerator CreateMarchingCubeMesh(Action<Mesh> callback)
        {
            Mesh mesh = new();
            yield return MarchingCubeMesher.Create(scalarField).Build(ret => mesh = ret);

            // MarchingCubeMesher.ApplyContourLineColor(scalarField, mesh);
            MeshRenderer.material = caveMaterial;

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
