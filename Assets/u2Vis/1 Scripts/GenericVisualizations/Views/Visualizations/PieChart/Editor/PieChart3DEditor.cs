using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(PieChart3D))]
    public class PieChart3DEditor : BaseVisualizationViewEditor
    {
        protected override void DrawGUIItems()
        {
            EditorGUILayout.PropertyField(showAxes_prop, new GUIContent("Show Axes"));
            if (showAxes_prop.boolValue)
                EditorGUILayout.PropertyField(axisViewPrefab_prop, new GUIContent("AxisView Prefab"));
            EditorGUILayout.PropertyField(style_prop, new GUIContent("Visualization Style"));
            EditorGUILayout.PropertyField(size_prop, new GUIContent("Size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_segments"), new GUIContent("Number of Segments"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_valueIndex"), new GUIContent("Index for Pie"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_3DIndex"), new GUIContent("Index for height"));
        }
    }
}
