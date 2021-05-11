using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(RevolvedCharts))]
    public class RevolvedChartsEditor : BaseVisualizationViewEditor
    {
        protected StackedBar _stackedBar = null;

        protected SerializedProperty
            multiDimPresenter_prop,
            dataItemMesh_prop,
            usedCategorical_prop,
            dimIndex_prop;

        protected override void OnEnable()
        {
            base.OnEnable();
            multiDimPresenter_prop = serializedObject.FindProperty("_multiDimPresenter");
            dataItemMesh_prop = serializedObject.FindProperty("_dataItemMesh");
            usedCategorical_prop = serializedObject.FindProperty("_usedCategorical");
            dimIndex_prop = serializedObject.FindProperty("_dimIndex");

            _stackedBar = (StackedBar)serializedObject.targetObject;
        }

        protected override void DrawGUIItems()
        {
            base.DrawGUIItems();
            EditorGUILayout.PropertyField(usedCategorical_prop, new GUIContent("Use categorical"));
            if (usedCategorical_prop.boolValue)
                EditorGUILayout.PropertyField(dimIndex_prop, new GUIContent("Dim.-index for categ."));
        }
    }
}
