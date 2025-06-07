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

        public void GenerateMarchingCubeMesh()
        {
            Mesh mesh = CreateMarchingCubeMesh();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
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
