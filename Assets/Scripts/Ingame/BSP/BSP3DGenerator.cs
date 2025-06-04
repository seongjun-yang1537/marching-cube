using System;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using JetBrains.Annotations;
using MCube;
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
        private static int MIN_CHILD_SIZE = 10;
        private static int MAX_SIZE = 30;

        private static MT19937 rng = MT19937.Create();
        private static void PartitionNode(BSP3DTreeNode node, int depth = 0)
        {
            node.depth = depth;
            node.childs = new();

            BSP3DCube cube = node.cube;

            if (!cube.size.ToArray().Any(len => len >= MAX_SIZE) && rng.NextFloat() < 0.1 * depth)
            {
                return;
            }
            if (depth >= 10)
            {
                return;
            }

            List<int> partitionAxes = cube.size.ToArray()
            .Select((len, idx) => new { len = len, idx = idx })
            .Where(data => data.len >= 2 * MIN_CHILD_SIZE)
            .Select(data => data.idx)
            .ToList();

            if (partitionAxes.Count == 0)
                return;
            int partitionAxis = partitionAxes.Choice(rng);

            BSP3DPlane separator = new();


            switch (partitionAxis)
            {
                case 0:
                    {
                        int vx = rng.NextInt(cube.topLeft.x + MIN_CHILD_SIZE, cube.bottomRight.x - MIN_CHILD_SIZE);

                        separator = new BSP3DPlane(
                            new Vector3Int(vx, cube.topLeft.y, cube.topLeft.z),
                            new Vector3Int(vx, cube.bottomRight.y, cube.bottomRight.z)
                        );
                    }
                    break;
                case 1:
                    {
                        int vy = rng.NextInt(cube.topLeft.y + MIN_CHILD_SIZE, cube.bottomRight.y - MIN_CHILD_SIZE);

                        separator = new BSP3DPlane(
                            new Vector3Int(cube.topLeft.x, vy, cube.topLeft.z),
                            new Vector3Int(cube.bottomRight.x, vy, cube.bottomRight.z)
                        );
                    }
                    break;
                case 2:
                    {
                        int vz = rng.NextInt(cube.topLeft.z + MIN_CHILD_SIZE, cube.bottomRight.z - MIN_CHILD_SIZE);

                        separator = new BSP3DPlane(
                            new Vector3Int(cube.topLeft.x, cube.topLeft.y, vz),
                            new Vector3Int(cube.bottomRight.x, cube.bottomRight.y, vz)
                        );
                    }
                    break;
            }

            node.separator = separator;

            Vector3Int child0TL = cube.topLeft;
            Vector3Int child0BR = separator.bottomRight;
            Vector3Int child1TL = separator.topLeft;
            Vector3Int chidl1BR = cube.bottomRight;

            BSP3DTreeNode child0 = new(child0TL, child0BR);
            BSP3DTreeNode child1 = new(child1TL, chidl1BR);

            node.childs = new() { child0, child1 };

            PartitionNode(child0, depth + 1);
            PartitionNode(child1, depth + 1);
        }

        private static void GenerateRoomAndGraph(BSP3DMapAsset config)
        {
            List<BSP3DTreeNode> leafs = config.GetLeafs();
            foreach (var leaf in leafs)
            {
                if (rng.NextFloat() < 0.5f)
                {
                    leaf.GenerateRoom(rng);
                }
            }
        }

        private static void GenerateGraph(BSP3DMapAsset config)
        {
            BSP3DGraph graph = new(config.GetLeafs()
                .Select(leaf => new BSP3DGraphNode(leaf))
                .ToList());
            graph.GenerateAdjacencyGraph();

            config.graph = graph;
        }

        private static void GenerateRoomGraph(BSP3DMapAsset config)
        {
            BSP3DGraph graph = config.graph;
            BSP3DGraph roomGraph = config.roomGraph;

            List<BSP3DTreeNode> leafs = config.GetLeafs();
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

            List<int> distance = new List<int>().Resize(leafCount, -1);
            List<int> prev = new List<int>().Resize(leafCount, -1);

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

                    distance[edge.to] = distance[top] + (activate[edge.to] ? 1 : 0);
                    prev[edge.to] = edge.from;

                    if (activate[edge.to])
                        deque.PushBack(edge.to);
                    else
                        deque.PushFront(edge.to);
                }
            }

            List<int> destinations = graph
                .GetComponents(idx => leafs[idx].room != null)
                .Select(list => list.Choice(rng))
                .ToList();

            List<int> addIndices = new();
            foreach (int dest in destinations)
            {
                int now = dest;
                while (now != startIndex)
                {
                    if (!activate[now]) addIndices.Add(now);
                    now = prev[now];
                }
            }
            addIndices = addIndices.Distinct().ToList();

            foreach (int idx in addIndices) leafs[idx].GenerateJunctionRoom(rng);

            roomGraph = new(config.GetRoomNodes()
                .Select(leaf => new BSP3DGraphNode(leaf))
                .ToList());
            roomGraph.GenerateAdjacencyGraph();

            roomGraph.SetEdges(roomGraph.GetRandomSpanningTree());

            config.roomGraph = roomGraph;
        }

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

        private static void GenerateScalarGrid(BSP3DMapAsset mapAsset)
        {
            ScalarField CreateScalarField(Vector3Int size)
            {
                ScalarField scalarField = ScriptableObject.CreateInstance<ScalarField>();
                scalarField.Resize(size);
                scalarField.threshold = Mathf.Epsilon;
                return scalarField;
            }

            ScalarField scalarField = CreateScalarField(mapAsset.size);
            scalarField.All(1.0f);

            List<BSP3DRoom> rooms = mapAsset.GetRoomNodes().Select(node => node.room).ToList();
            foreach (BSP3DRoom room in rooms)
            {
                CIterator.GetArray3D(room.size)
                .Select(pos => room.topLeft + pos)
                .ForEach(idx => scalarField[idx] = 0.0f);
            }
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

            Vector3Int startPos = scalarField.Indices
                .Where(idx => Mathf.Approximately(scalarField[idx], 0.0f))
                .ToList()
                .Choice(rng);

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
            mapAsset.scalarField = scalarField;

            BSP3DScalarGrid scalarGrid = new(scalarField);
            mapAsset.scalarGrid = scalarGrid;
        }

        private static void CellularAutomate(BSP3DMapAsset config)
        {
            ScalarField scalarField = config.scalarField;
            foreach (Vector3Int idx in scalarField.Indices)
            {
                if (Mathf.Approximately(scalarField[idx], 0.0f)) continue;
                scalarField[idx] = (rng.NextFloat() >= 0.75f ? 0.0f : 1.0f);
            }

            float ratio = 0.4f;
            void relax()
            {
                ScalarField now = config.scalarField;
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

            int count = 2;
            while (count-- > 0) relax();
        }

        public static void Generate(BSP3DMapAsset config)
        {
            rng = MT19937.Create();

            config.InitializeRoot();
            PartitionNode(config.root);
            GenerateRoomAndGraph(config);
            GenerateGraph(config);
            GenerateRoomGraph(config);
            GenerateScalarGrid(config);
            CellularAutomate(config);

            config.scalarField.AddPadding(1, 1.0f);
        }
    }
}