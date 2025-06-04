using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutInt : SUIElement
    {
        private string prefix;
        private int value;
        private int? minValue;
        private int? maxValue;
        private UnityAction<int> onValueChanged;

        public SEditorGUILayoutInt(string prefix, int value)
        {
            this.prefix = prefix;
            this.value = value;
        }

        public SEditorGUILayoutInt OnValueChanged(UnityAction<int> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutInt Min(int minValue)
        {
            this.minValue = minValue;
            return this;
        }

        public SEditorGUILayoutInt Max(int maxValue)
        {
            this.maxValue = maxValue;
            return this;
        }

        public SEditorGUILayoutInt Clamp(int left, int right)
        {
            this.minValue = left;
            this.maxValue = right;
            return this;
        }

        public override void Render()
        {
            int newValue = EditorGUILayout.IntField(prefix, value);
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