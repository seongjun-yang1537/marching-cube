using System;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Ingame
{
    [Serializable]
    public class BSP3DGenerationParam
    {
        public Vector3Int size;
        public int maxDepth;
        public Vector3Int minCellSize = -Vector3Int.one;
        public Vector3Int maxCellSize = -Vector3Int.one;
        public Vector3 minCellSizeRatio = -Vector3.one;
        public Vector3 maxCellSizeRatio = -Vector3.one;

        [SerializeField]
        private BSP3DProbabilityCurve _splitByDepthCurve;
        // 깊이 별 더 쪼갤 확률
        public BSP3DProbabilityCurve splitByDepthCurve
        {
            get => FetchSplitByDepthCurve();
        }

        [SerializeField]
        public BSP3DProbabilityVector3Curve _roomSizeRatioCurve;
        // 생성되는 방 크기 확률
        public BSP3DProbabilityVector3Curve roomSizeRatioCurve
        {
            get => FetchRoomSizeRatioCurve();
        }

        [SerializeField]
        public BSP3DProbabilityVector3Curve _splitLengthRatioCurve;
        // 분할하는 길이의 확률
        public BSP3DProbabilityVector3Curve splitLengthRatioCurve
        {
            get => FetchSplitLengthRatioCurve();
        }

        [SerializeField]
        private BSP3DProbabilityCurve _roomByDepthCurve;
        // 깊이 별 방 생성 확률
        public BSP3DProbabilityCurve roomByDepthCurve
        {
            get => FetchRoomByDepthCurve();
        }

        public List<int> ToArrayMinCellSize() => minCellSize.Flatten();

        public List<int> ToArrayMaxCellSize()
        {
            List<int> sizies = size.Flatten();
            List<int> maxs = maxCellSize.Flatten().Select((v, idx) => v == -1 ? sizies[idx] : v).ToList();

            return maxs;
        }

        public (List<int>, List<int>) ToArrayCellSize() => (ToArrayMinCellSize(), ToArrayMaxCellSize());

        public List<float> ToArrayMinCellSizeRatio() => minCellSizeRatio.Flatten();

        public List<float> ToArrayMaxCellSizeRatio()
            => minCellSizeRatio.Flatten().Select((v, idx) => v == -1 ? 1.0f : v).ToList();

        public (List<float>, List<float>) ToArrayCellSizeRatio()
            => (ToArrayMinCellSizeRatio(), ToArrayMaxCellSizeRatio());

        private BSP3DProbabilityCurve FetchSplitByDepthCurve()
        {
            List<float> units = Enumerable.Range(0, maxDepth).Select(depth => (float)depth).ToList();
            _splitByDepthCurve ??= new BSP3DProbabilityCurve(units);
            return _splitByDepthCurve.ReUnit(units);
        }

        private BSP3DProbabilityVector3Curve FetchRoomSizeRatioCurve()
        {
            _roomSizeRatioCurve ??= new();

            List<float> units = Enumerable.Range(0, 11).Select(unit => unit / 10.0f).ToList();
            _roomSizeRatioCurve.xCurve ??= new BSP3DProbabilityCurve(units);
            _roomSizeRatioCurve.yCurve ??= new BSP3DProbabilityCurve(units);
            _roomSizeRatioCurve.zCurve ??= new BSP3DProbabilityCurve(units);

            _roomSizeRatioCurve.xCurve.ReUnit(units);
            _roomSizeRatioCurve.yCurve.ReUnit(units);
            _roomSizeRatioCurve.zCurve.ReUnit(units);

            return _roomSizeRatioCurve;
        }

        private BSP3DProbabilityVector3Curve FetchSplitLengthRatioCurve()
        {
            _splitLengthRatioCurve ??= new();

            List<float> units = Enumerable.Range(0, 11).Select(unit => unit / 10.0f).ToList();
            _splitLengthRatioCurve.xCurve ??= new BSP3DProbabilityCurve(units);
            _splitLengthRatioCurve.yCurve ??= new BSP3DProbabilityCurve(units);
            _splitLengthRatioCurve.zCurve ??= new BSP3DProbabilityCurve(units);

            _splitLengthRatioCurve.xCurve.ReUnit(units);
            _splitLengthRatioCurve.yCurve.ReUnit(units);
            _splitLengthRatioCurve.zCurve.ReUnit(units);

            return _splitLengthRatioCurve;
        }

        private BSP3DProbabilityCurve FetchRoomByDepthCurve()
        {
            List<float> units = Enumerable.Range(0, maxDepth).Select(depth => (float)depth).ToList();

            _roomByDepthCurve ??= new BSP3DProbabilityCurve(units);
            return _roomByDepthCurve.ReUnit(units);
        }
    }
}