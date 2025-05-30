using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MCube
{
    [CustomEditor(typeof(ScalarFieldModel))]
    public class EditorScalarFieldModel : Editor
    {
        private Editor scalarFieldEditor;

        ScalarFieldModel script;

        void OnEnable()
        {
            script = (ScalarFieldModel)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();
            Inspector_ScalarFieldScriptableObject();

            serializedObject.ApplyModifiedProperties();
        }

        bool showScalarFieldScriptableObject;
        private void Inspector_ScalarFieldScriptableObject()
        {
            if (!script.scalarField)
                return;


            { EditorGUILayout.BeginVertical("helpbox"); }
            {
                { GUILayout.BeginHorizontal(); GUILayout.Space(15); }
                {
                    showScalarFieldScriptableObject = EditorGUILayout.BeginFoldoutHeaderGroup(
                                    showScalarFieldScriptableObject, "Scalar Field Inspector");
                }
                { GUILayout.EndHorizontal(); }
            }
            { EditorGUILayout.EndVertical(); }

            if (showScalarFieldScriptableObject)
            {
                { EditorGUILayout.BeginVertical("helpbox"); }
                {
                    if (scalarFieldEditor == null || scalarFieldEditor.target != script.scalarField)
                    {
                        scalarFieldEditor = CreateEditor(script.scalarField);
                    }

                    scalarFieldEditor.OnInspectorGUI();
                }
                { EditorGUILayout.EndVertical(); }
            }

            { EditorGUILayout.EndFoldoutHeaderGroup(); }
        }
    }

}
