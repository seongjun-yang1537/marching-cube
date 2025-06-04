using System;
using UnityEngine;

namespace Ingame
{
    [Serializable]
    public class BSP3DGenerationParam
    {
        public int maxDepth;
        public Vector3Int minCellSize = -Vector3Int.one;
        public Vector3Int maxCellSize = -Vector3Int.one;
        public Vector3 minCellSizeRatio = -Vector3.one;
        public Vector3 maxCellSizeRatio = -Vector3.one;

        [SerializeField]
        public BSP3DProbabilityCurve splitByDepthCurve;
        [SerializeField]
        public BSP3DProbabilityVector3Curve cellSizeRatioCurve;
        [SerializeField]
        public BSP3DProbabilityVector3Curve splitLengthRatioCurve;
        [SerializeField]
        public BSP3DProbabilityCurve roomByDepthCurve;
    }
}