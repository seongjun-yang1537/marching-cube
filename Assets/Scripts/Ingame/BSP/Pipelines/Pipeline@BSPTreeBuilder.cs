using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class BSPTreeBuilder : IBSP3DGenerationStep
    {
        private static int MAX_SIZE = 30;
        private int SelectPartitionAxis(MT19937 rng, BSP3DCube cube, BSP3DGenerationParam param)
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

        private BSP3DPlane GetSeparatorPlane(MT19937 rng, BSP3DCube cube, int axis, BSP3DGenerationParam param)
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

        private void PartitionNode(MT19937 rng, BSP3DTreeNode node, BSP3DGenerationParam param, int depth = 0)
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

            int partitionAxis = SelectPartitionAxis(rng, node.cube, param);
            if (partitionAxis == -1)
                return;

            BSP3DPlane separator = GetSeparatorPlane(rng, node.cube, partitionAxis, param);
            node.separator = separator;

            Vector3Int child0TL = cube.topLeft;
            Vector3Int child0BR = separator.bottomRight;
            Vector3Int child1TL = separator.topLeft;
            Vector3Int chidl1BR = cube.bottomRight;

            BSP3DTreeNode childL = new(child0TL, child0BR);
            BSP3DTreeNode childR = new(child1TL, chidl1BR);

            node.childs = new() { childL, childR };

            PartitionNode(rng, childL, param, depth + 1);
            PartitionNode(rng, childR, param, depth + 1);
        }

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
            mapAsset.InitializeRoot();
            PartitionNode(rng, mapAsset.root, mapAsset.param);
            yield return null;
        }
    }
}