using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(Scatterplot2D))]
    public class ScatterPlotEditor : BaseVisualizationViewEditor
    {
        protected Scatterplot2D _scatterPlot2D = null;

        protected SerializedProperty
            displayRelativeValues_prop,
            zoomMin_prop,
            zoomMax_prop;

        protected override void OnEnable()
        {
            base.OnEnable();
            displayRelativeValues_prop = serializedObject.FindProperty("_displayRelativeValues");
            zoomMin_prop = serializedObject.FindProperty("_zoomMin");
            zoomMax_prop = serializedObject.FindProperty("_zoomMax");

            _scatterPlot2D = (Scatterplot2D)serializedObject.targetObject;
        }

        protected override void DrawGUIItems()
        {
            base.DrawGUIItems();
            EditorGUILayout.PropertyField(displayRelativeValues_prop, new GUIContent("Display Relative Values"));
            DrawZoomLevelGUI();
        }

        protected virtual void DrawZoomLevelGUI()
        {
            var centerAligned = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            var rightAligned = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };

            EditorGUILayout.LabelField("Zoom Level", EditorStyles.boldLabel);
            var min = zoomMin_prop.vector3Value;
            var max = zoomMax_prop.vector3Value;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(min.x.ToString("0.00"));
            GUILayout.Label("X-Axis", centerAligned);
            GUILayout.Label(max.x.ToString("0.00"), rightAligned);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref min.x, ref max.x, 0, 1);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(min.y.ToString("0.00"));
            GUILayout.Label("Y-Axis", centerAligned);
            GUILayout.Label(max.y.ToString("0.00"), rightAligned);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref min.y, ref max.y, 0, 1);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(min.z.ToString("0.00"));
            GUILayout.Label("Z-Axis", centerAligned);
            GUILayout.Label(max.z.ToString("0.00"), rightAligned);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref min.z, ref max.z, 0, 1);

            if (min != zoomMin_prop.vector3Value)
                zoomMin_prop.vector3Value = min;
            if (max != zoomMax_prop.vector3Value)
                zoomMax_prop.vector3Value = max;
        }

    }
}
