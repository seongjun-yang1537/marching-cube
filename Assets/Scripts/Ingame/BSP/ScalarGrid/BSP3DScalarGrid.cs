using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Corelib.Utils;
using MCube;
using System.Linq;
using Sirenix.Utilities;

namespace Ingame
{
    public class BSP3DScalarGrid
    {
        public Vector3Int size;
        public Vector3Int gridSize { get => ((Vector3)size / 16.0f).CeilToInt(); }
        public int gridLength { get => gridSize.x * gridSize.y * gridSize.z; }

        public List<ScalarField> chunks;

        public BSP3DScalarGrid()
        {

        }
        public BSP3DScalarGrid(ScalarField scalarField)
        {
            size = scalarField.size;
        }

        public int ToGrid1D(int x, int y, int z)
            => x + (y * gridSize.x) + (z * gridSize.x * gridSize.y);

        public int ToGrid1D(Vector3Int coords)
            => coords.x + (coords.y * gridSize.x) + (coords.z * gridSize.x * gridSize.y);

        public Vector3Int ToGrid3DCoord(int index)
        {
            int z = index / (gridSize.x * gridSize.y);
            int remaining = index % (gridSize.x * gridSize.y);
            int y = remaining / gridSize.x;
            int x = remaining % gridSize.x;

            return new Vector3Int(x, y, z);
        }
    }
}
