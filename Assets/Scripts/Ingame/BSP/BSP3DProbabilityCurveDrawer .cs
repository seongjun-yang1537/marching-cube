using UnityEditor;
using UnityEngine;

namespace Ingame
{
    [CustomPropertyDrawer(typeof(BSP3DProbabilityCurve))]
    public class BSP3DProbabilityCurveDrawer : PropertyDrawer
    {
        private const float LabelHeight = 20f;
        private const float GraphHeight = 100f;
        private const float XLabelHeight = 16f;
        private const float Spacing = 4f;
        private const float MinSliderWidth = 10f;
        private const float YAxisLabelWidth = 30f;
        private const int YTicks = 5;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LabelHeight + GraphHeight + XLabelHeight + Spacing * 5;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty unitsProp = property.FindPropertyRelative("units");
            SerializedProperty valueProp = property.FindPropertyRelative("value");

            EditorGUI.HelpBox(new Rect(position.x, position.y, position.width, LabelHeight), label.text, MessageType.None);
            position.y += LabelHeight + Spacing + 5;

            int count = unitsProp.arraySize;
            if (count == 0)
            {
                EditorGUI.LabelField(position, "(No Data)");
                return;
            }

            float availableWidth = position.width - YAxisLabelWidth;
            float sliderWidth = Mathf.Max(MinSliderWidth, (availableWidth - (count - 1) * Spacing) / count);
            float totalContentWidth = sliderWidth * count + Spacing * (count - 1);
            float startX = position.x + YAxisLabelWidth;
            if (totalContentWidth < availableWidth)
                startX += (availableWidth - totalContentWidth) * 0.5f;

            float graphX = startX;
            float graphY = position.y;
            float graphWidth = totalContentWidth;
            float graphHeight = GraphHeight;

            Vector3[] curvePoints = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                float val = valueProp.GetArrayElementAtIndex(i).floatValue;
                float centerX = graphX + i * (sliderWidth + Spacing) + sliderWidth * 0.5f;
                float y = graphY + (1f - val) * graphHeight;
                curvePoints[i] = new Vector3(centerX, y, 0f);
            }

            Handles.BeginGUI();
            EditorGUI.DrawRect(new Rect(graphX, graphY, graphWidth, graphHeight), new Color(0f, 0f, 0f, 0.2f));
            Handles.color = new Color(1f, 1f, 1f, 0.15f);

            for (int i = 0; i <= YTicks; i++)
            {
                float t = i / (float)YTicks;
                float y = graphY + (1f - t) * graphHeight;
                Handles.DrawLine(new Vector3(graphX, y), new Vector3(graphX + graphWidth, y));
                GUI.Label(new Rect(position.x, y - 8f, YAxisLabelWidth - 2f, 16f), t.ToString("0.0"), EditorStyles.miniLabel);
            }

            Handles.color = new Color(0f, 1f, 1f, 0.6f);
            Handles.DrawAAPolyLine(3f, curvePoints);
            Handles.EndGUI();

            for (int i = 0; i < count; i++)
            {
                SerializedProperty valElement = valueProp.GetArrayElementAtIndex(i);
                float centerX = graphX + i * (sliderWidth + Spacing) + sliderWidth * 0.5f;
                Rect sliderRect = new Rect(centerX, graphY, sliderWidth, graphHeight);

                EditorGUI.BeginChangeCheck();
                float newValue = GUI.VerticalSlider(sliderRect, valElement.floatValue, 1f, 0f);
                if (EditorGUI.EndChangeCheck())
                    valElement.floatValue = Mathf.Clamp01(newValue);
            }

            for (int i = 0; i < count; i++)
            {
                SerializedProperty unitElement = unitsProp.GetArrayElementAtIndex(i);
                float centerX = graphX + i * (sliderWidth + Spacing) + sliderWidth * 0.5f;
                float labelY = graphY + graphHeight + Spacing;
                GUI.Label(new Rect(centerX - 15f, labelY, 30f, XLabelHeight), unitElement.floatValue.ToString("0.0"), EditorStyles.miniLabel);
            }
        }
    }
}