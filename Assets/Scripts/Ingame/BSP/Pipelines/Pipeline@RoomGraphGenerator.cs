using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class RoomGraphGenerator : IBSP3DGenerationStep
    {
        private static List<int> GetTrackingBridge(
            MT19937 rng,
            BSP3DMapAsset mapAsset,
            List<int> prev
        )
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

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
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

            List<int> addIndices = GetTrackingBridge(rng, mapAsset, prev)
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

            yield return null;
        }
    }
}