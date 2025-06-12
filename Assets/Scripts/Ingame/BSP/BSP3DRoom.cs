using System;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public enum BSP3DRoomType
    {
        None,
        Start,
        Portal,
    }
    [Serializable]
    public class BSP3DRoom : BSP3DCube
    {
        public int idx;
        public BSP3DRoomType roomType = BSP3DRoomType.None;

        [Serializable]
        public class ProjectionGridDictionary : SerializableDictionary<BSP3DCubeFace, BSP3DProjectionGrid> { }
        [SerializeField]
        public ProjectionGridDictionary projectionGrid = new();

        public List<BSP3DProjectionGrid> ProjectionGrids { get => projectionGrid.Values.ToList(); }

        public BSP3DRoom() : base()
        {

        }
        public BSP3DRoom(Vector3Int topLeft, Vector3Int bottomRight) : base(topLeft, bottomRight)
        {
        }

        public void InitializeProjectionGrind(List<Vector3Int> poses)
        {
            projectionGrid = new();

            foreach (BSP3DCubeFace face in Enum.GetValues(typeof(BSP3DCubeFace)))
            {
                var plane = GetFace(face);
                projectionGrid.Add(face, new BSP3DProjectionGrid(center.RoundToInt(), plane, poses));
            }
        }

        public Vector3Int SelectSpawnPosition(MT19937 rng = null)
        {
            BSP3DProjectionGrid grid = projectionGrid[BSP3DCubeFace.BOTTOM];
            if (grid.validCount == 0) return -Vector3Int.one;

            if (rng == null) rng = MT19937.Create();
            return grid.validPoses.Choice(rng);
        }
    }
}