using System;
using UnityEngine;
namespace MCube
{
    public static class ScalarFieldGenerator
    {
        public static ScalarField Random(ScalarField scalarField)
        {
            scalarField.ClearField();

            uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
            MT19937 mt = new MT19937(seed);

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

            uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
            MT19937 mt = new MT19937(seed);
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