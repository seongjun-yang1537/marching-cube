using System;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace Ingame
{
    public class BSP3DGraphEdgeBuilder
    {
        private int from;
        private int to;
        private int idx = -1;
        private BSP3DCube plane;

        public BSP3DGraphEdgeBuilder SetEdge(int from, int to)
        {
            this.from = from;
            this.to = to;
            return this;
        }

        public BSP3DGraphEdgeBuilder SetIndex(int idx)
        {
            this.idx = idx;
            return this;
        }

        public BSP3DGraphEdgeBuilder SetPlane(BSP3DCube plane)
        {
            this.plane = plane;
            return this;
        }

        public BSP3DGraphEdge Build()
        {
            var edge = new BSP3DGraphEdge(from, to, idx);
            edge.plane = plane;
            return edge;
        }
    }

    [Serializable]
    public class BSP3DGraphEdge
    {
        public int from, to, idx;
        [SerializeField]
        public BSP3DCube plane;

        public BSP3DGraphEdge(int from, int to, int idx = -1)
        {
            this.from = from;
            this.to = to;
            this.idx = idx;
        }
    }

    [Serializable]
    public class BSP3DGraphNode : BSP3DCube
    {
        public int idx;

        public BSP3DGraphNode() : base()
        {

        }
        public BSP3DGraphNode(Vector3Int topLeft, Vector3Int bottomRight)
         : base(topLeft, bottomRight)
        {
        }
        public BSP3DGraphNode(BSP3DCube other) : this(other.topLeft, other.bottomRight)
        {

        }

        public BSP3DGraphNode(BSP3DTreeNode treeNode) : this(treeNode.cube)
        {

        }
    }

    [Serializable]
    public class BSP3DGraphCSR
    {
        public int nodeCount;
        public int edgeCount;

        public List<BSP3DGraphEdge> edges;

        [SerializeField]
        public List<int> cnt;
        [SerializeField]
        public List<BSP3DGraphEdge> csr;

        public BSP3DGraphCSR(List<BSP3DGraphEdge> edges)
        {
            GenerateCSR(edges);
        }

        private void GenerateCSR(List<BSP3DGraphEdge> edges)
        {
            this.edges = new(edges);
            edgeCount = edges.Count;
            nodeCount = edges.Count == 0 ? 0 : edges.Max(e => Math.Max(e.from, e.to)) + 1;

            cnt = new List<int>().Resize(nodeCount + 2, 0);
            csr = new List<BSP3DGraphEdge>().Resize(edgeCount);

            foreach (BSP3DGraphEdge edge in edges)
                cnt[edge.from + 1]++;
            for (int i = 1; i < cnt.Count; i++)
                cnt[i] += cnt[i - 1];

            List<int> temp = new(cnt);
            foreach (BSP3DGraphEdge edge in edges)
                csr[temp[edge.from]++] = edge;
        }

        public List<BSP3DGraphEdge> Adjust(int idx)
        {
            List<BSP3DGraphEdge> adjusts = new();
            for (int i = cnt[idx]; i < cnt[idx + 1]; i++)
                adjusts.Add(csr[i]);
            return adjusts;
        }
    }

    [Serializable]
    public class BSP3DGraph
    {
        public int nodeCount { get => nodes.Count; }
        public int edgeCount { get => edges.Count; }

        public List<BSP3DGraphNode> nodes;
        public List<BSP3DGraphEdge> edges;

        public BSP3DGraphCSR csr;

        public BSP3DGraph()
        {

        }
        public BSP3DGraph(List<BSP3DGraphNode> nodes) : this()
        {
            this.nodes = nodes;
        }

        public void SetEdges(List<BSP3DGraphEdge> edges)
        {
            this.edges = edges;
            this.csr = new BSP3DGraphCSR(edges);
        }

        public List<BSP3DGraphEdge> GetAdjacencyEdges()
        {
            List<BSP3DGraphEdge> edges = new();
            for (int i = 0; i < nodeCount; i++)
                for (int j = i + 1; j < nodeCount; j++)
                {
                    BSP3DCube adjacentPlane = nodes[i].GetAdjacentRegion(nodes[j]);

                    if (adjacentPlane == null)
                        continue;

                    edges.Add(new BSP3DGraphEdgeBuilder()
                        .SetEdge(i, j)
                        .SetIndex(edges.Count)
                        .SetPlane(adjacentPlane)
                        .Build());
                    edges.Add(new BSP3DGraphEdgeBuilder()
                        .SetEdge(j, i)
                        .SetIndex(edges.Count)
                        .SetPlane(adjacentPlane)
                        .Build());
                }
            return edges;
        }

        public void GenerateAdjacencyGraph()
        {
            SetEdges(GetAdjacencyEdges());
        }

        public void GenerateCorridorBetween(int from, int to)
        {

        }

        public void DFS(int pos, ref List<bool> visit)
        {
            List<int> component = new();
            DFS(pos, ref visit, ref component);
        }

        public void DFS(int pos, ref List<bool> visit, ref List<int> component)
            => DFS(pos, ref visit, ref component, idx => true);

        public void DFS(int pos, ref List<bool> visit, ref List<int> component, Func<int, bool> func)
        {
            visit[pos] = true;
            component.Add(pos);

            foreach (BSP3DGraphEdge edge in Adjust(pos))
                if (!visit[edge.to] && func(edge.to))
                    DFS(edge.to, ref visit, ref component, func);
        }

        public List<List<int>> GetComponents(Func<int, bool> func)
        {
            List<List<int>> components = new();
            List<bool> visit = new List<bool>().Resize(nodeCount);

            for (int i = 0; i < nodeCount; i++)
            {
                if (visit[i] || !func(i)) continue;
                List<int> component = new();
                DFS(i, ref visit, ref component, func);
                components.Add(component);
            }

            return components;
        }
        public List<List<int>> GetComponents() => GetComponents(idx => true);

        public List<BSP3DGraphEdge> Adjust(int idx) => csr.Adjust(idx);

        public List<BSP3DGraphEdge> GetRandomSpanningTree(MT19937 rng = null)
        {
            List<BSP3DGraphEdge> shuffleEdges =
                new List<BSP3DGraphEdge>(edges).Shuffle(rng);

            List<BSP3DGraphEdge> spanningTree = new();
            UnionFind unionFind = new(nodeCount);
            foreach (BSP3DGraphEdge edge in shuffleEdges)
                if (unionFind.Merge(edge.from, edge.to))
                    spanningTree.Add(edge);

            return spanningTree;
        }
    }
}