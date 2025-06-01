using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutToggle : SUIElement
    {
        private string label;
        private bool value;
        private UnityAction<bool> onValueChanged;

        public SEditorGUILayoutToggle(string label, bool value)
        {
            this.label = label;
            this.value = value;
        }

        public SEditorGUILayoutToggle OnValueChanged(UnityAction<bool> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public override void Render()
        {
            bool newValue = EditorGUILayout.Toggle(label, value);
            if (newValue != value)
            {
                value = newValue;
                onValueChanged?.Invoke(newValue);
            }
        }
    }
}