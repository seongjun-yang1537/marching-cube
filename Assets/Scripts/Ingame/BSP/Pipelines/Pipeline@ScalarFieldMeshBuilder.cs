using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using MCube;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class ScalarFieldMeshBuilder : IBSP3DGenerationStep
    {
        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            ScalarFieldModel scalarFieldModel = model.GetComponent<ScalarFieldModel>();
            scalarFieldModel.scalarField = model.mapAsset.scalarField;

            yield return scalarFieldModel.GenerateMarchingCubeMesh();
        }
    }
}