using System;
using NUnit.Framework.Internal.Builders;
using UnityEditor;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayout : SUIElement
    {
        protected SEditorGUILayout(UnityAction onRender) : base(onRender)
        {
        }

        public static SUIElement Label(string text) =>
            new SEditorGUILayout(() => EditorGUILayout.LabelField(text));

        public static SEditorGUILayoutButton Button(string label)
            => new SEditorGUILayoutButton(label);

        public static SEditorGUILayoutHorizontal Horizontal(SUIElement content = null) =>
            new SEditorGUILayoutHorizontal(content);

        public static SEditorGUILayoutVertical Vertical(SUIElement content = null) =>
            new SEditorGUILayoutVertical(content);

        public static SEditorGUILayoutToggle Toggle(string label, bool value)
            => new SEditorGUILayoutToggle(label, value);
    }
}