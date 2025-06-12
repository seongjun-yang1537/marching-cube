using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using MCube;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ingame
{
    [CreateAssetMenu(fileName = "New BSP3D Map Asset", menuName = "ScriptableObject/BSP3D Map Asset")]
    public class BSP3DMapAsset : SerializedScriptableObject
    {
        [HideInInspector]
        public BSP3DTreeNode root;

        public BSP3DGraph graph;
        public BSP3DGraph roomGraph;

        public ScalarField scalarField; // DEBUG
        public BSP3DScalarGrid scalarGrid;

        public Vector3Int size
        {
            get => param.size;
            set => param.size = value;
        }

        [SerializeField]
        public BSP3DGenerationParam param;

        public int maxDepth;
        public int minCellSize, maxCellSize;

        public BSP3DProbabilityCurve depthSplitCurve;
        public BSP3DProbabilityCurve cellSizeCurve;

        public void OnEnable()
        {
            InitliazeCurve();
        }

        public void InitliazeCurve()
        {
            depthSplitCurve.ReUnit(Enumerable.Range(0, maxDepth).Select(value => 1.0f * value).ToList());
            cellSizeCurve.ReUnit(Enumerable.Range(0, 10).Select(value => value / 10.0f).ToList());
        }

        public void InitializeRoot()
        {
            root = new BSP3DTreeNode(Vector3Int.zero, size);
        }

        public BSP3DRoom FindPortalRoom()
            => GetRooms().Find(room => room.roomType == BSP3DRoomType.Portal);
        public BSP3DRoom FindStartRoom()
            => GetRooms().Find(room => room.roomType == BSP3DRoomType.Start);

        public List<BSP3DTreeNode> GetNodes() => root.ToList();
        public List<BSP3DTreeNode> GetLeafs() => GetNodes()
            .Where(node => node.isLeaf)
            .ToList();

        public List<BSP3DTreeNode> GetRoomNodes() => GetLeafs()
            .Where(node => node.room != null)
            .ToList();

        public List<BSP3DRoom> GetRooms() => GetRoomNodes().Select(node => node.room).ToList();
    }
}