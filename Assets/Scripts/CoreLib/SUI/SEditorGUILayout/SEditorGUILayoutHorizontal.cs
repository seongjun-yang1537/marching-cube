using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutHorizontal : SUIElement
    {
        private readonly SUIElement content;
        private string style = "";

        public SEditorGUILayoutHorizontal()
        {

        }

        public SEditorGUILayoutHorizontal(SUIElement content)
        {
            this.content = content;
        }

        public SEditorGUILayoutHorizontal Style(string style)
        {
            this.style = style;
            return this;
        }

        public override void Render()
        {
            EditorGUILayout.BeginHorizontal(style);
            content?.Render();
            EditorGUILayout.EndHorizontal();
        }
    }
}