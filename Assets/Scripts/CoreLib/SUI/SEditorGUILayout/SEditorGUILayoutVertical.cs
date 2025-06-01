using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutVertical : SUIElement
    {
        private SUIElement content;
        private string style = "";

        public SEditorGUILayoutVertical()
        {

        }

        public SEditorGUILayoutVertical(SUIElement content)
        {
            this.content = content;
        }

        public SEditorGUILayoutVertical Style(string style)
        {
            this.style = style;
            return this;
        }

        public override void Render()
        {
            EditorGUILayout.BeginVertical(style);
            content?.Render();
            EditorGUILayout.EndVertical();
        }
    }
}