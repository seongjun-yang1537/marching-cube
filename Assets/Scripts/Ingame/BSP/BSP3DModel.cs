using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.SUI;
using Corelib.Utils;
using MCube;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

            [Serializable]
            public class ProjectionGridDictionary : SerializableDictionary<BSP3DCubeFace, bool> { }

            [SerializeField]
            private ProjectionGridDictionary _visibleRoomProjectionGrid;

            public ProjectionGridDictionary VisibleRoomProjectionGrid
            {
                get
                {
                    if (_visibleRoomProjectionGrid == null || _visibleRoomProjectionGrid.Count == 0)
                    {
                        _visibleRoomProjectionGrid = new();
                        foreach (BSP3DCubeFace face in Enum.GetValues(typeof(BSP3DCubeFace)))
                            _visibleRoomProjectionGrid[face] = false;
                    }
                    return _visibleRoomProjectionGrid;
                }
            }
        }
        [SerializeField]
        public GizmosOption gizmosOption;

        public BSP3DMapAsset mapAsset;
        public int seed;

        private ScalarFieldModel _scalarFieldModel;
        private ScalarFieldModel ScalarFieldModel
        {
            get => _scalarFieldModel ??= GetComponent<ScalarFieldModel>();
        }

        public IEnumerator GenerateAsync(BSP3DGenerationContext context)
        {
            context.model = this;

            transform.DestroyAllChildrenWithEditor();
            yield return BSP3DGenerator.GenerateAsync(context);


            ScalarFieldModel.scalarField = mapAsset.scalarField;
            yield return ScalarFieldModel.GenerateMarchingCubeMesh();
        }

        public BSP3DRoom FindPortalRoom() => mapAsset.FindPortalRoom();
        public BSP3DRoom FindStartRoom() => mapAsset.FindStartRoom();

        public void OnDrawGizmos()
        {
            if (!mapAsset)
                return;

            DrawGizmosBSP();
            DrawGizmosProjectionGrids();
        }

        private void DrawGizmosBSP()
        {
            SGizmos.Scope(
                SGizmos.Action(() => DrawGizmosBSPArea(mapAsset.root))
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
                SGizmos.WireCube(mapAsset.size / 2, mapAsset.size)
            )
            .Matrix(transform.localToWorldMatrix)
            .Render();


            float depthRatio = 1.0f * depth / mapAsset.maxDepth;

            SGizmos.Scope(
                SGizmos.WireCube(node.cube.center, node.cube.size - Vector3.one * depthRatio)
            )
            .Color(new Color().Rainbow(depthRatio))
            .Render();

            foreach (var child in node.childs)
                DrawGizmosBSPArea(child, depth + 1);
        }

        private void DrawGizmosRoom(BSP3DRoom room)
        {
            switch (room.roomType)
            {
                case BSP3DRoomType.None:
                    {
                        SGizmos.Scope(
                            SGizmos.Cube(room.center, room.size)
                        )
                        .Render();
                    }
                    break;
                case BSP3DRoomType.Start:
                    {
                        SGizmos.Scope(
                            SGizmos.Cube(room.center, room.size)
                        )
                        .Color(Color.red)
                        .Render();
                    }
                    break;
                case BSP3DRoomType.Portal:
                    {
                        SGizmos.Scope(
                            SGizmos.Cube(room.center, room.size)
                        )
                        .Color(Color.blue)
                        .Render();
                    }
                    break;
            }
        }

        private void DrawGizmosRoom()
        {
            if (!gizmosOption.visibleRoom)
                return;

            List<BSP3DTreeNode> leafs = mapAsset.GetRoomNodes();
            foreach (BSP3DTreeNode leaf in leafs)
            {
                DrawGizmosRoom(leaf.room);
            }
        }

        private void DrawGraph()
        {
            if (!gizmosOption.visibleRoomGraph)
                return;

            BSP3DGraph graph = mapAsset.roomGraph;

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

        private void DrawGizmosProjectionGrids()
        {
            List<BSP3DRoom> rooms = mapAsset.GetRooms();
            foreach (BSP3DRoom room in rooms)
            {
                DrawGizmosProjectionGrid(room);
            }
        }

        private void DrawGizmosProjectionGrid(BSP3DRoom room)
        {
            SGizmos.Scope(
                SGizmos.Action(() =>
                {
                    foreach (BSP3DCubeFace face in Enum.GetValues(typeof(BSP3DCubeFace)))
                    {
                        if (!room.projectionGrid.ContainsKey(face) || !gizmosOption.VisibleRoomProjectionGrid[face])
                            continue;

                        BSP3DProjectionGrid grid = room.projectionGrid[face];

                        CIterator.GetArray2D(grid.size)
                            .Where(pos => grid[pos] != -Vector3Int.one)
                            .ForEach(pos => SGizmos.Cube(grid[pos], Vector3.one).Render());
                    }
                })
            )
            .Matrix(transform.localToWorldMatrix)
            .Render();
        }

        public List<BSP3DRoom> GetRoomsByType(BSP3DRoomType type)
            => mapAsset.GetRooms().Where(room => room.roomType == type).ToList();
    }
}

