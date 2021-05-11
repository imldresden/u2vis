using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(BarChart3D))]
    public class BarChart3DEditor : BaseVisualizationViewEditor
    {
        protected BarChart3D _barChart3D = null;

        protected SerializedProperty
            multiDimPresenter_prop,
            dataItemMesh_prop,
            barThickness_prop;

        protected override void OnEnable()
        {
            base.OnEnable();
            multiDimPresenter_prop = serializedObject.FindProperty("_multiDimPresenter");
            dataItemMesh_prop = serializedObject.FindProperty("_dataItemMesh");
            barThickness_prop = serializedObject.FindProperty("_barThickness");

            _barChart3D = (BarChart3D)serializedObject.targetObject;
        }

        protected override void DrawGUIItems()
        {
            base.DrawGUIItems();
            EditorGUILayout.PropertyField(dataItemMesh_prop, new GUIContent("Data Item Mesh"));
            EditorGUILayout.PropertyField(barThickness_prop, new GUIContent("Bar Thickness"));
        }
    }
}
