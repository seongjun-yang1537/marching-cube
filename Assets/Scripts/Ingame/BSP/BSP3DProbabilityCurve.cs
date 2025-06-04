using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
namespace Ingame
{
    [Serializable]
    public class BSP3DProbabilityCurve
    {
        [SerializeField]
        public List<float> units;
        [SerializeField]
        private List<float> value;
        public float minUnit;
        public float maxUnit;
        public BSP3DProbabilityCurve(List<float> units)
        {
            this.units = new();
            this.value = new();
            ReUnit(units);
        }

        public void ReUnit(List<float> newUnits)
        {
            units = newUnits;
            if (units.Count == 0)
                return;
            minUnit = units.Min();
            maxUnit = units.Max();

            List<float> oldValues = value;
            int minSize = units.Count;

            value = new List<float>(minSize);
            for (int i = 0; i < minSize; i++)
            {
                if (oldValues != null && i < oldValues.Count)
                    value.Add(Mathf.Clamp01(oldValues[i]));
                else
                    value.Add(0.5f);
            }
        }

        public void RemoveUnit(int idx) => units.RemoveAt(idx);
        public void InsertUnit(int idx, float t) => units.Insert(idx, t);

        public float this[int idx]
        {
            get => units[idx];
            set => units[idx] = Mathf.Clamp(value, 0f, 1f);
        }
    }

    [Serializable]
    public class BSP3DProbabilityVector3Curve
    {
        [SerializeField]
        public BSP3DProbabilityCurve xCurve;
        [SerializeField]
        public BSP3DProbabilityCurve yCurve;
        [SerializeField]
        public BSP3DProbabilityCurve zCurve;

        public void ReUnit(List<float> newUnits)
        {
            xCurve.ReUnit(newUnits);
            yCurve.ReUnit(newUnits);
            zCurve.ReUnit(newUnits);
        }

        public void RemoveUnit(int idx)
        {
            xCurve.RemoveUnit(idx);
            yCurve.RemoveUnit(idx);
            zCurve.RemoveUnit(idx);
        }
        public void InsertUnit(int idx, float t)
        {
            xCurve.InsertUnit(idx, t);
            yCurve.InsertUnit(idx, t);
            zCurve.InsertUnit(idx, t);
        }

        public Vector3 this[int idx]
        {
            get => new Vector3(xCurve[idx], yCurve[idx], zCurve[idx]);
            set
            {
                xCurve[idx] = value.x;
                yCurve[idx] = value.y;
                zCurve[idx] = value.z;
            }
        }
    }
}