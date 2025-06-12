using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class MapGraphGenerator : IBSP3DGenerationStep
    {

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
            BSP3DGraph graph = new(mapAsset.GetLeafs()
                .Select(leaf => new BSP3DGraphNode(leaf))
                .ToList());
            graph.GenerateAdjacencyGraph();

            mapAsset.graph = graph;
            yield return null;
        }
    }
}