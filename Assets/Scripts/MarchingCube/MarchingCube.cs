using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MCube
{
    public static class MarchingCube
    {
        // Vertex layout:
        //
        //            6             7
        //            +-------------+   
        //          / |           / | 
        //        /   |         /   |    
        //    2 +-----+-------+  3  |   
        //      |   4 +-------+-----+ 5   
        //      |   /         |   /     
        //      | /           | /   
        //    0 +-------------+ 1      
        //
        // Triangulation cases are generated prioritising rotations over inversions, which can introduce non-manifold geometry.
        public static List<Vector3> GetTriangle(List<bool> weights)
        {
            Debug.Assert(weights.Count == 8);

            int idx = 0;
            for (int i = 0; i < 8; i++)
            {
                if (weights[i])
                {
                    idx |= 1 << i;
                }
            }

            List<Vector3> triangles = new();
            int[] indices = MarchingCubeTable.TriangleTable[idx];
            for (int i = 0; i <= indices.Length - 3; i += 3)
            {
                int a0 = MarchingCubeTable.EdgeVertexIndices[indices[i], 0];
                int a1 = MarchingCubeTable.EdgeVertexIndices[indices[i], 1];

                int b0 = MarchingCubeTable.EdgeVertexIndices[indices[i + 1], 0];
                int b1 = MarchingCubeTable.EdgeVertexIndices[indices[i + 1], 1];

                int c0 = MarchingCubeTable.EdgeVertexIndices[indices[i + 2], 0];
                int c1 = MarchingCubeTable.EdgeVertexIndices[indices[i + 2], 1];

                Vector3 a = ((Vector3)MarchingCubeTable.Delta[a0] + (Vector3)MarchingCubeTable.Delta[a1]) * 0.5f;
                Vector3 b = ((Vector3)MarchingCubeTable.Delta[b0] + (Vector3)MarchingCubeTable.Delta[b1]) * 0.5f;
                Vector3 c = ((Vector3)MarchingCubeTable.Delta[c0] + (Vector3)MarchingCubeTable.Delta[c1]) * 0.5f;

                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(b);
            }

            return triangles;
        }
    }
}