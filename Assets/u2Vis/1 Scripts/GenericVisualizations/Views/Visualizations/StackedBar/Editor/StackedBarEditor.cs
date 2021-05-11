using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(StackedBar))]
    public class StackedBarEditor : BaseVisualizationViewEditor
    {
        protected StackedBar _stackedBar = null;

        protected SerializedProperty
            multiDimPresenter_prop,
            dataItemMesh_prop,
            useMinItem_prop;

        protected override void OnEnable()
        {
            base.OnEnable();
            multiDimPresenter_prop = serializedObject.FindProperty("_multiDimPresenter");
            dataItemMesh_prop = serializedObject.FindProperty("_dataItemMesh");
            useMinItem_prop = serializedObject.FindProperty("_useMinIndex");

            _stackedBar = (StackedBar)serializedObject.targetObject;
        }

        protected override void DrawGUIItems()
        {
            base.DrawGUIItems();
            EditorGUILayout.PropertyField(dataItemMesh_prop, new GUIContent("Data Item Mesh"));
            EditorGUILayout.PropertyField(useMinItem_prop, new GUIContent("Use MinItem"));
            if(!useMinItem_prop.boolValue)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_valueIndex"), new GUIContent("Index for Stack"));
        }
    }
}
