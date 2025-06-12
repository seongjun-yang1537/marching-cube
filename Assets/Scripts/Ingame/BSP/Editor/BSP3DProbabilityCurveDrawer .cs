using Corelib.SUI;
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

        public static void Draw(BSP3DProbabilityCurve curve)
        {
            if (curve == null || curve.units == null || curve.UnitsCount == 0 || curve.ValueCount != curve.UnitsCount)
            {
                EditorGUILayout.LabelField("(Invalid or No Curve Data)");
                return;
            }

            int count = curve.UnitsCount;

            Rect graphAreaRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(GraphHeight));

            float availableWidthForGraphAndSliders = graphAreaRect.width - YAxisLabelWidth;
            float sliderWidth = Mathf.Max(MinSliderWidth, (availableWidthForGraphAndSliders - (count - 1) * Spacing) / count);
            float totalContentWidthForSliders = sliderWidth * count + Spacing * (count - 1);

            float graphContentStartX = graphAreaRect.x + YAxisLabelWidth;
            if (totalContentWidthForSliders < availableWidthForGraphAndSliders)
            {
                graphContentStartX += (availableWidthForGraphAndSliders - totalContentWidthForSliders) * 0.5f;
            }

            for (int i = 0; i <= YTicks; i++)
            {
                float t = i / (float)YTicks;
                float yPos = graphAreaRect.y + (1f - t) * GraphHeight;
                GUI.Label(new Rect(graphAreaRect.x, yPos - 8f, YAxisLabelWidth - 2f, 16f), t.ToString("0.0"), EditorStyles.miniLabel);
            }

            Rect actualGraphDrawingRect = new Rect(graphContentStartX, graphAreaRect.y, totalContentWidthForSliders, GraphHeight);

            Vector3[] curvePoints = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                float val = Mathf.Clamp01(curve.GetValue(i)); // GetValue 사용
                float centerX = actualGraphDrawingRect.x + i * (sliderWidth + Spacing) + sliderWidth * 0.5f;
                float y = actualGraphDrawingRect.y + (1f - val) * actualGraphDrawingRect.height;
                float x = centerX - sliderWidth * 0.5f;
                curvePoints[i] = new Vector3(x, y, 0f);
            }

            Handles.BeginGUI();
            EditorGUI.DrawRect(actualGraphDrawingRect, new Color(0f, 0f, 0f, 0.2f));
            Handles.color = new Color(1f, 1f, 1f, 0.15f);

            for (int i = 0; i <= YTicks; i++)
            {
                float t = i / (float)YTicks;
                float yPos = actualGraphDrawingRect.y + (1f - t) * actualGraphDrawingRect.height;
                Handles.DrawLine(new Vector3(actualGraphDrawingRect.x, yPos), new Vector3(actualGraphDrawingRect.xMax, yPos));
            }

            Handles.color = new Color(0f, 1f, 1f, 0.6f);
            Handles.DrawAAPolyLine(3f, curvePoints);
            Handles.EndGUI();

            for (int i = 0; i < count; i++)
            {
                Rect sliderRect = new Rect(actualGraphDrawingRect.x + i * (sliderWidth + Spacing), actualGraphDrawingRect.y, sliderWidth, actualGraphDrawingRect.height);

                EditorGUI.BeginChangeCheck();
                float newValue = GUI.VerticalSlider(sliderRect, curve.GetValue(i), 1f, 0f); // GetValue 사용
                if (EditorGUI.EndChangeCheck())
                {
                    // curve.SetValue(i, newValue)를 호출하기 전에 Undo를 기록해야 할 수 있습니다.
                    // 예: if (myUnityEngineObject != null) Undo.RecordObject(myUnityEngineObject, "Modify Curve Value");
                    curve.SetValue(i, newValue); // SetValue 사용
                    // 예: if (myUnityEngineObject != null) EditorUtility.SetDirty(myUnityEngineObject);
                }
            }

            Rect xLabelAreaRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(XLabelHeight));
            for (int i = 0; i < count; i++)
            {
                float unitVal = curve.units[i]; // units는 public이므로 직접 접근
                float labelCenterX = graphContentStartX + i * (sliderWidth + Spacing) + sliderWidth * 0.5f;
                GUI.Label(new Rect(labelCenterX - 15f, xLabelAreaRect.y, 30f, XLabelHeight), unitVal.ToString("0.0"), EditorStyles.miniLabel);
            }
            EditorGUILayout.Space(Spacing);
        }

        public static void DrawVector3(BSP3DProbabilityVector3Curve vectorCurves)
        {
            SEditorGUILayout.Vertical("helpbox")
            .Content(
                SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Label("X").Bold()
                )
                + SGUILayout.Space(10)
                + SEditorGUILayout.Action(() => Draw(vectorCurves.xCurve))

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Label("Y").Bold()
                )
                + SGUILayout.Space(10)
                + SEditorGUILayout.Action(() => Draw(vectorCurves.yCurve))

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Label("Z").Bold()
                )
                + SGUILayout.Space(10)
                + SEditorGUILayout.Action(() => Draw(vectorCurves.zCurve))
            )
            .Render();
        }
    }
}