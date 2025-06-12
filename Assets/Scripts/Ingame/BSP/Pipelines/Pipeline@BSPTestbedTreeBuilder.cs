using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class BSPTestbedTreeBuilder : IBSP3DGenerationStep
    {
        private void PartitionNode(MT19937 rng, BSP3DTreeNode node, BSP3DGenerationParam param, int depth = 0)
        {
            node.depth = depth;
            node.childs = new();
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