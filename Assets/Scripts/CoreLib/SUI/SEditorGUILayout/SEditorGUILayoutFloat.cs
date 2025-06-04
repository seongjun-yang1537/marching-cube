using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutFloat : SUIElement
    {
        private string prefix;
        private float value;
        private float? minValue;
        private float? maxValue;
        private UnityAction<float> onValueChanged;

        public SEditorGUILayoutFloat(string prefix, float value)
        {
            this.prefix = prefix;
            this.value = value;
        }

        public SEditorGUILayoutFloat OnValueChanged(UnityAction<float> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutFloat Min(float minValue)
        {
            this.minValue = minValue;
            return this;
        }

        public SEditorGUILayoutFloat Max(float maxValue)
        {
            this.maxValue = maxValue;
            return this;
        }

        public SEditorGUILayoutFloat Clamp(float left, float right)
        {
            this.minValue = left;
            this.maxValue = right;
            return this;
        }

        public override void Render()
        {
            float newValue = EditorGUILayout.FloatField(prefix, value);
            if (minValue != null) newValue = Math.Max(minValue.Value, newValue);
            if (maxValue != null) newValue = Math.Min(newValue, maxValue.Value);

            if (newValue != value)
            {
                value = newValue;
                onValueChanged?.Invoke(newValue);
            }
        }
    }
}