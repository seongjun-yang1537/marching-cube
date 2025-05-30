using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MCube
{
    public class ScalarFieldModel : MonoBehaviour
    {
        public ScalarField scalarField;
        [Range(0f, 1f)]
        public float threshold;

        private Matrix4x4 trMat
        {
            get => Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        }
        public void OnDrawGizmosSelected()
        {
            if (!scalarField)
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
