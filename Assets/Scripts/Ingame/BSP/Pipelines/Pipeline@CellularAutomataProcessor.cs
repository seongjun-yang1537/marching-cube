using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using MCube;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class CellularAutomataProcessor : IBSP3DGenerationStep
    {
        private static void RelaxAutomata(BSP3DMapAsset mapAsset, float ratio = 0.4f)
        {

            ScalarField now = mapAsset.scalarField;
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

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
            ScalarField scalarField = mapAsset.scalarField;
            foreach (Vector3Int idx in scalarField.Indices)
            {
                if (Mathf.Approximately(scalarField[idx], 0.0f)) continue;
                scalarField[idx] = (rng.NextFloat() >= 0.75f ? 0.0f : 1.0f);
            }

            int count = 2;
            while (count-- > 0)
            {
                yield return null;
                RelaxAutomata(mapAsset, 0.4f);
            }
        }
    }
}