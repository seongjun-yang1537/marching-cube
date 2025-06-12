using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class TestbedRoomGenerator : IBSP3DGenerationStep
    {

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
            List<BSP3DTreeNode> leafs = mapAsset.GetLeafs();
            foreach (var leaf in leafs)
            {
                leaf.GenerateRoom(rng);
                yield return null;
            }
        }
    }
}