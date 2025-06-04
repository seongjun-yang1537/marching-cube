using System;
using Corelib.Utils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace MCube
{
    public static class ScalarFieldGenerator
    {
        public static ScalarField Random(ScalarField scalarField)
        {
            scalarField.ClearField();

            MT19937 mt = MT19937.Create();

            foreach (Vector3Int i in scalarField.Indices)
            {
                scalarField[i] = mt.NextFloat();
            }

            return scalarField;
        }

        public static ScalarField PerlinNoise(ScalarField scalarField, float scale = 0.1f)
        {
            scalarField.ClearField();
            Vector3Int size = scalarField.size;

            MT19937 mt = MT19937.Create();
            float seedX = mt.NextFloat();
            float seedZ = mt.NextFloat();

            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    float yRatio = Mathf.PerlinNoise(
                        x * scale + seedX,
                        z * scale + seedZ
                    );
                    for (int y = 0; y < size.y * yRatio; y++)
                    {
                        scalarField[new Vector3Int(x, y, z)] = 1;
                    }
                }
            return scalarField;
        }
    }
}