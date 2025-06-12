using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using MCube;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class ScalaFieldPostProcessor : IBSP3DGenerationStep
    {
        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
            mapAsset.scalarField.AddPadding(1, 1.0f);
            yield return null;
        }
    }
}