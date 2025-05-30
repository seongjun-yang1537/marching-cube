using UnityEngine;
using UnityEditor;

namespace MCube
{
    [CustomEditor(typeof(ScalarField))]
    public class EditorScalarField : Editor
    {
        ScalarField script;

        Vector3Int inputNewSize;
        void OnEnable()
        {
            script = (ScalarField)target;

            inputNewSize = script.size;
        }

        public override void OnInspectorGUI()
        {
            Inspect_Size();
        }

        private void Inspect_Size()
        {
            { EditorGUILayout.BeginVertical("Box"); }
            {
                EditorGUILayout.Vector3IntField("Size", script.size);
            }
            { EditorGUILayout.EndVertical(); }

            inputNewSize = EditorGUILayout.Vector3IntField("New Size", inputNewSize);
            if (script.IsLimitSize(inputNewSize))
            {
                EditorGUILayout.HelpBox(
                                $"Size {inputNewSize} exceeds limit of total elements.",
                                MessageType.Warning
                            );
            }

            if (GUILayout.Button("Resize"))
            {
                if (!script.IsLimitSize(inputNewSize) && script.size != inputNewSize)
                {
                    script.Resize(inputNewSize);
                }
            }

            Inspector_Generator();
        }

        private void Inspector_Generator()
        {
            { EditorGUILayout.BeginVertical("helpbox"); }
            {
                if (GUILayout.Button("Generate Random"))
                {
                    script.GenerateRandom();
                }
                if (GUILayout.Button("Generate Perlin Noise"))
                {
                    script.GeneratePerlinNoise();
                }
            }
            { EditorGUILayout.EndVertical(); }
        }
    }

}