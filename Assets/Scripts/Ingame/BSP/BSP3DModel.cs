using System;
using System.Collections;
using System.Collections.Generic;
using Corelib.SUI;
using Corelib.Utils;
using MCube;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Ingame
{
    public class BSP3DModel : SerializedMonoBehaviour
    {
        [Serializable]
        public class GizmosOption
        {
            public bool visibleBSPArea = true;
            public bool visibleRoom = true;
            public bool visibleRoomGraph = true;
        }
        [SerializeField]
        public GizmosOption gizmosOption;

        public BSP3DMapAsset config;

        public void Generate()
        {
            BSP3DGenerator.Generate(config);
            ScalarFieldModel scalarFieldModel = GetComponent<ScalarFieldModel>();
            scalarFieldModel.scalarField = config.scalarField;
            scalarFieldModel.GenerateMarchingCubeMesh();
        }

        public void OnDrawGizmos()
        {
            DrawGizmosBSP();
        }

        private void DrawGizmosBSP()
        {
            SGizmos.Scope(
                SGizmos.Action(() => DrawGizmosBSPArea(config.root))
                + SGizmos.Action(() => DrawGizmosRoom())
                + SGizmos.Action(() => DrawGraph())
            )
            .Matrix(transform.localToWorldMatrix)
            .Render();
        }

        private void DrawGizmosBSPArea(BSP3DTreeNode node, int depth = 0)
        {
            if (!gizmosOption.visibleBSPArea)
                return;

            SGizmos.Scope(
                SGizmos.WireCube(config.size / 2, config.size)
            )
            .Matrix(transform.localToWorldMatrix)
            .Render();


            float depthRatio = 1.0f * depth / config.maxDepth;

            SGizmos.Scope(
                SGizmos.WireCube(node.cube.center, node.cube.size - Vector3.one * depthRatio)
            )
            .Color(new Color().Rainbow(depthRatio))
            .Render();

            foreach (var child in node.childs)
                DrawGizmosBSPArea(child, depth + 1);
        }

        private void DrawGizmosRoom()
        {
            if (!gizmosOption.visibleRoom)
                return;

            List<BSP3DTreeNode> leafs = config.GetRoomNodes();
            foreach (BSP3DTreeNode leaf in leafs)
            {
                BSP3DRoom room = leaf.room;
                SGizmos.Cube(room.center, room.size)
                .Render();
            }
        }

        private void DrawGraph()
        {
            if (!gizmosOption.visibleRoomGraph)
                return;

            BSP3DGraph graph = config.roomGraph;

            foreach (BSP3DGraphNode node in graph.nodes)
            {
                SGizmos.Scope(
                    SGizmos.Sphere(node.center, 2.5f)
                )
                .Color(Color.yellow)
                .Render();
            }

            foreach (BSP3DGraphEdge edge in graph.edges)
            {
                (int u, int v) = (edge.from, edge.to);
                SGizmos.Scope(
                    SGizmos.Line(graph.nodes[u].center, graph.nodes[v].center)
                    + SGizmos.Cube(edge.plane.center, edge.plane.size)
                )
                .Color(Color.yellow)
                .Render();
            }
        }
    }
}

