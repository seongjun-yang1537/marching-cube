using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Corelib.Utils;
using JetBrains.Annotations;
using MCube;
using Microsoft.Unity.VisualStudio.Editor;
using Palmmedia.ReportGenerator.Core;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ingame
{
    public class BSP3DGenerator
    {
        private static class BSPTreeBuilder
        {
            private static MT19937 rng = null;

            private static int SelectPartitionAxis(BSP3DCube cube, BSP3DGenerationParam param)
            {
                var mins = param.ToArrayMinCellSize();

                List<int> partitionAxes = cube.size.ToArray()
                .Select((len, idx) => new { len = len, idx = idx })
                .Where(data => data.len >= 2 * mins[data.idx])
                .Select(data => data.idx)
                .ToList();
                if (partitionAxes.Count == 0)
                    return -1;
                return partitionAxes.Choice(rng);
            }

            private static BSP3DPlane GetSeparatorPlane(BSP3DCube cube, int axis, BSP3DGenerationParam param)
            {
                int delta = Math.Max(
                    param.minCellSize.GetAt(axis),
                    Mathf.RoundToInt(cube.size.GetAt(axis) * param.minCellSizeRatio.GetAt(axis))
                );
                int center = rng.NextInt(cube.topLeft.GetAt(axis) + delta, cube.bottomRight.GetAt(axis) - delta);

                Vector3Int topLeft = cube.topLeft;
                Vector3Int bottomRight = cube.bottomRight;

                return new BSP3DPlane(
                    topLeft.SetAt(axis, center),
                    bottomRight.SetAt(axis, center)
                );
            }

            private static void PartitionNode(BSP3DTreeNode node, BSP3DGenerationParam param, int depth = 0)
            {
                node.depth = depth;
                node.childs = new();

                BSP3DCube cube = node.cube;

                // TODO
                if (!cube.size.ToArray().Any(len => len >= MAX_SIZE) && rng.NextFloat() < 0.1 * depth)
                {
                    return;
                }
                if (depth >= param.maxDepth)
                {
                    return;
                }

                int partitionAxis = SelectPartitionAxis(node.cube, param);
                if (partitionAxis == -1)
                    return;

                BSP3DPlane separator = GetSeparatorPlane(node.cube, partitionAxis, param);
                node.separator = separator;

                Vector3Int child0TL = cube.topLeft;
                Vector3Int child0BR = separator.bottomRight;
                Vector3Int child1TL = separator.topLeft;
                Vector3Int chidl1BR = cube.bottomRight;

                BSP3DTreeNode childL = new(child0TL, child0BR);
                BSP3DTreeNode childR = new(child1TL, chidl1BR);

                node.childs = new() { childL, childR };

                PartitionNode(childL, param, depth + 1);
                PartitionNode(childR, param, depth + 1);
            }

            public static void Build(MT19937 rng, BSP3DMapAsset mapAsset)
            {
                BSPTreeBuilder.rng = rng;
                mapAsset.InitializeRoot();
                PartitionNode(mapAsset.root, mapAsset.param);
            }
        }

        private static class RoomGenerator
        {
            private static MT19937 rng = null;
            public static void Generate(MT19937 rng, BSP3DMapAsset mapAsset)
            {
                RoomGenerator.rng = rng;
                List<BSP3DTreeNode> leafs = mapAsset.GetLeafs();
                foreach (var leaf in leafs)
                {
                    if (rng.NextFloat() < 0.5f)
                    {
                        leaf.GenerateRoom(rng);
                    }
                }
            }
        }

        private static class MapGraphGenerator
        {
            private static MT19937 rng = null;

            public static void Generate(MT19937 rng, BSP3DMapAsset mapAsset)
            {
                MapGraphGenerator.rng = rng;

                BSP3DGraph graph = new(mapAsset.GetLeafs()
                    .Select(leaf => new BSP3DGraphNode(leaf))
                    .ToList());
                graph.GenerateAdjacencyGraph();

                mapAsset.graph = graph;
            }
        }

        private static class RoomGraphGenerator
        {
            private static MT19937 rng = null;

            private static List<int> GetTrackingBridge(BSP3DMapAsset mapAsset, List<int> prev)
            {
                BSP3DGraph graph = mapAsset.graph;
                List<BSP3DTreeNode> leafs = mapAsset.GetLeafs();

                List<int> destinations = graph
                    .GetComponents(idx => leafs[idx].room != null)
                    .Select(list => list.Choice(rng))
                    .ToList();

                List<int> indices = new();
                foreach (int dest in destinations)
                {
                    int now = dest;
                    while (now != -1)
                    {
                        indices.Add(now);
                        now = prev[now];
                    }
                }

                return indices;
            }

            private static (List<int> distance, List<int> prev) BFS01(BSP3DGraph graph, int startIndex, Func<int, int> cost)
            {
                int count = graph.nodeCount;

                List<int> distance = new List<int>().Resize(count, -1);
                List<int> prev = new List<int>().Resize(count, -1);

                Deque<int> deque = new();
                deque.PushBack(startIndex);
                distance[startIndex] = 0;

                while (deque.Count > 0)
                {
                    int top = deque.PopFront();

                    List<BSP3DGraphEdge> adjust = graph.Adjust(top);
                    foreach (var edge in adjust)
                    {
                        if (distance[edge.to] != -1)
                            continue;

                        distance[edge.to] = distance[top] + cost(edge.to);
                        prev[edge.to] = edge.from;

                        if (distance[edge.to] == distance[top])
                            deque.PushBack(edge.to);
                        else
                            deque.PushFront(edge.to);
                    }
                }

                return (distance, prev);
            }

            public static void Generate(MT19937 rng, BSP3DMapAsset mapAsset)
            {
                RoomGraphGenerator.rng = rng;

                BSP3DGraph graph = mapAsset.graph;
                BSP3DGraph roomGraph = mapAsset.roomGraph;

                List<BSP3DTreeNode> leafs = mapAsset.GetLeafs();
                int leafCount = leafs.Count;

                List<bool> activate = new List<bool>().Resize(leafCount, false);
                for (int i = 0; i < leafCount; i++)
                    if (leafs[i].room != null) activate[i] = true;

                int startIndex = leafs
                    .Select((leaf, idx) => new { leaf = leaf, idx = idx })
                    .Where(data => data.leaf.room != null)
                    .Select(data => data.idx)
                    .ToList()
                    .Choice(rng);

                var (distance, prev) = BFS01(graph, startIndex, (idx) => activate[idx] ? 1 : 0);

                List<int> addIndices = GetTrackingBridge(mapAsset, prev)
                    .Where(idx => !activate[idx])
                    .Distinct()
                    .ToList();

                foreach (int idx in addIndices) leafs[idx].GenerateJunctionRoom(rng);

                roomGraph = new(mapAsset.GetRoomNodes()
                    .Select(leaf => new BSP3DGraphNode(leaf))
                    .ToList());
                roomGraph.GenerateAdjacencyGraph();

                roomGraph.SetEdges(roomGraph.GetRandomSpanningTree());

                mapAsset.roomGraph = roomGraph;
            }
        }

        private static class ScalarFieldGenerator
        {
            private class PQNode : IComparable<PQNode>
            {
                public Vector3Int pos;
                public int idx;
                public float cost;

                public PQNode(Vector3Int pos, int idx, float cost)
                {
                    this.pos = pos;
                    this.idx = idx;
                    this.cost = cost;
                }

                public int CompareTo(PQNode other)
                {
                    return (int)(cost - other.cost);
                }
            }

            private static MT19937 rng = null;

            private static ScalarField CreateScalarField(Vector3Int size)
            {
                ScalarField scalarField = ScriptableObject.CreateInstance<ScalarField>();
                scalarField.Resize(size);
                scalarField.threshold = Mathf.Epsilon;
                return scalarField;
            }

            private static void CarveBSPRooms(BSP3DMapAsset mapAsset, ScalarField scalarField)
            {
                List<BSP3DRoom> rooms = mapAsset.GetRoomNodes().Select(node => node.room).ToList();
                foreach (BSP3DRoom room in rooms)
                {
                    CIterator.GetArray3D(room.size)
                    .Select(pos => room.topLeft + pos)
                    .ForEach(idx => scalarField[idx] = 0.0f);
                }
            }

            private static void MappingThresholdPerlingNoise(ScalarField scalarField)
            {
                Vector3 seedVector = new Vector3(rng.NextFloat(), rng.NextFloat(), rng.NextFloat()) * 10;
                scalarField.ThreshodlMap(idx =>
                {
                    Vector3 weight = seedVector + idx;
                    return (
                        Mathf.PerlinNoise(weight.x, weight.y) +
                        Mathf.PerlinNoise(weight.y, weight.z) +
                        Mathf.PerlinNoise(weight.x, weight.z)
                    ) / 3;
                });
            }

            private static (List<float> distance, List<int> prev) Dijkstra(ScalarField scalarField, Vector3Int startPos)
            {
                List<float> distance = new List<float>().Resize(scalarField.count, 0x3f3f3f3f);
                List<int> prev = new List<int>().Resize(scalarField.count, -1);
                PriorityQueue<PQNode> pq = new();
                pq.Enqueue(new PQNode(startPos, scalarField.GetIndex(startPos), 0.0f));
                distance[scalarField.GetIndex(startPos)] = 0.0f;

                while (pq.Count > 0)
                {
                    PQNode top = pq.Dequeue();
                    if (distance[top.idx] > top.cost) continue;

                    for (int i = 1; i <= 2; i++)
                    {
                        foreach (Vector3Int delta in ExVector3Int.DIR6)
                        {
                            Vector3Int to = top.pos + delta * i;
                            if (!scalarField.InRange(to)) continue;

                            int toIdx = scalarField.GetIndex(to);
                            float cost = top.cost + scalarField[to];
                            if (distance[toIdx] > cost)
                            {
                                distance[toIdx] = cost;
                                prev[toIdx] = top.idx;
                                pq.Enqueue(new PQNode(to, toIdx, cost));
                            }
                        }
                    }
                }

                return (distance, prev);
            }

            private static void CarveTrackingBridge(ScalarField scalarField, List<int> prev)
            {
                List<Vector3Int> destPoses = scalarField
                    .GetComponents(idx => Mathf.Approximately(scalarField[idx], 0.0f))
                    .Select(poses => poses[0])
                    .ToList();

                foreach (Vector3Int dest in destPoses)
                {
                    int idx = scalarField.GetIndex(dest);
                    while (idx != -1)
                    {
                        Vector3Int pivot = scalarField.SpreadIndex(idx);
                        foreach (Vector3Int delta in ExVector3Int.DIR26)
                        {
                            Vector3Int to = pivot + delta;
                            if (!scalarField.InRange(to)) continue;
                            scalarField[to] = 0.0f;
                        }
                        idx = prev[idx];
                    }
                }
            }

            public static void Generate(MT19937 rng, BSP3DMapAsset mapAsset)
            {
                ScalarFieldGenerator.rng = rng;

                ScalarField scalarField = CreateScalarField(mapAsset.size);
                scalarField.All(1.0f);

                CarveBSPRooms(mapAsset, scalarField);
                MappingThresholdPerlingNoise(scalarField);

                Vector3Int startPos = scalarField.Indices
                    .Where(idx => Mathf.Approximately(scalarField[idx], 0.0f))
                    .ToList()
                    .Choice(rng);

                var (_, prev) = Dijkstra(scalarField, startPos);
                CarveTrackingBridge(scalarField, prev);

                mapAsset.scalarField = scalarField;

                BSP3DScalarGrid scalarGrid = new(scalarField);
                mapAsset.scalarGrid = scalarGrid;
            }
        }

        private static class CellularAutomataProcessor
        {
            private static MT19937 rng = null;

            private static void RelaxAutomata(BSP3DMapAsset mapAsset, float ratio = 0.4f)
            {

                ScalarField now = mapAsset.scalarField;
                ScalarField prev = new(now);

                foreach (Vector3Int idx in now.Indices)
                {
                    int count = 0, maxCount = 0;
                    foreach (Vector3Int delta in ExVector3Int.DIR27)
                    {
                        Vector3Int to = idx + delta;
                        if (!prev.InRange(to)) continue;
                        maxCount++;
                        if (Mathf.Approximately(prev[to], 0.0f)) count++;
                    }
                    now[idx] = (count >= ratio * maxCount ? 1.0f : 0.0f);
                }
            }

            public static void Process(MT19937 rng, BSP3DMapAsset mapAsset)
            {
                ScalarField scalarField = mapAsset.scalarField;
                foreach (Vector3Int idx in scalarField.Indices)
                {
                    if (Mathf.Approximately(scalarField[idx], 0.0f)) continue;
                    scalarField[idx] = (rng.NextFloat() >= 0.75f ? 0.0f : 1.0f);
                }

                int count = 2;
                while (count-- > 0) RelaxAutomata(mapAsset, 0.4f);
            }
        }

        private static int MAX_SIZE = 30;

        private static MT19937 rng = MT19937.Create();

        public static void Generate(BSP3DMapAsset mapAsset)
        {
            rng = MT19937.Create();

            BSPTreeBuilder.Build(rng, mapAsset);
            RoomGenerator.Generate(rng, mapAsset);
            MapGraphGenerator.Generate(rng, mapAsset);
            RoomGraphGenerator.Generate(rng, mapAsset);
            ScalarFieldGenerator.Generate(rng, mapAsset);
            CellularAutomataProcessor.Process(rng, mapAsset);

            mapAsset.scalarField.AddPadding(1, 1.0f);
        }
    }
}