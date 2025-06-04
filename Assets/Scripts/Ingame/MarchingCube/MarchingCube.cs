using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MCube
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
    public static class MarchingCube
    {
        public static List<Vector3Int> GetDelatPositions(Vector3Int pos)
            => MarchingCubeTable.Delta.Select(delta => pos + delta).ToList();
        public static List<int> GetEdgeList(int mask)
        {
            int edgeMask = MarchingCubeTable.EdgeMasks[mask];
            List<int> usedEdge = new();
            for (int i = 0; i < MarchingCubeTable.EDGE_COUNT; i++)
            {
                if ((edgeMask & (1 << i)) > 0)
                {
                    usedEdge.Add(i);
                }
            }
            return usedEdge;
        }
        public static IEnumerable<Vector2Int> GetEdgeIterator(int mask)
        {
            List<int> edges = GetEdgeList(mask);
            var indices = MarchingCubeTable.EdgeVertexIndices;
            return edges
                .Select(idx => new Vector2Int(indices[idx, 0], indices[idx, 1]));
        }
    }
}