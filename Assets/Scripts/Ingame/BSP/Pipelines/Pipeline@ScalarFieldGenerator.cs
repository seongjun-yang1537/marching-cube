using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using MCube;
using Sirenix.Utilities;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class ScalarFieldGenerator : IBSP3DGenerationStep
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

        private static IEnumerator MappingThresholdPerlingNoise(MT19937 rng, ScalarField scalarField)
        {
            Vector3 seedVector = new Vector3(rng.NextFloat(), rng.NextFloat(), rng.NextFloat()) * 10;

            int batchMax = 25, batch = 0;
            foreach (Vector3Int idx in scalarField.Indices)
            {
                if (!scalarField.IsSolid(idx)) continue;
                Vector3 weight = seedVector + idx;
                scalarField[idx] = (
                    Mathf.PerlinNoise(weight.x, weight.y) +
                    Mathf.PerlinNoise(weight.y, weight.z) +
                    Mathf.PerlinNoise(weight.x, weight.z)
                ) / 3;

                if (++batch == batchMax)
                {
                    batch = 0;
                    yield return null;
                }
            }
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

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
            ScalarField scalarField = CreateScalarField(mapAsset.size);
            scalarField.All(1.0f);

            CarveBSPRooms(mapAsset, scalarField);
            yield return MappingThresholdPerlingNoise(rng, scalarField);

            Vector3Int startPos = scalarField.Indices
                .Where(idx => Mathf.Approximately(scalarField[idx], 0.0f))
                .ToList()
                .Choice(rng);

            var (_, prev) = Dijkstra(scalarField, startPos);
            CarveTrackingBridge(scalarField, prev);

            mapAsset.scalarField = scalarField;

            BSP3DScalarGrid scalarGrid = new(scalarField);
            mapAsset.scalarGrid = scalarGrid;
            yield return null;
        }
    }
}