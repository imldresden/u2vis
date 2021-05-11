using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(BaseVisualizationViewEditor))]
    public abstract class BaseVisualizationViewEditor : Editor
    {
        protected SerializedProperty
            presenter_prop,
            size_prop,
            showAxes_prop,
            axisViewPrefab_prop,
            style_prop;

        protected BaseVisualizationView _baseView = null;

        protected virtual void OnEnable()
        {
            presenter_prop = serializedObject.FindProperty("_presenter");
            size_prop = serializedObject.FindProperty("_size");
            showAxes_prop = serializedObject.FindProperty("_showAxes");
            axisViewPrefab_prop = serializedObject.FindProperty("_axisViewPrefab");
            style_prop = serializedObject.FindProperty("_style");
            _baseView = (BaseVisualizationView)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawPresenterGUI();
            if (EditorGUI.EndChangeCheck())
                OnPresenterChanged();
            DrawGUIItems();

            serializedObject.ApplyModifiedProperties();
            if (_baseView.Presenter != null)
            {
                if (GUILayout.Button("Build Visualization"))
                    _baseView.Rebuild();
            }
            else
                EditorGUILayout.HelpBox("No Presenter has been set!", MessageType.Warning);
        }

        protected virtual void OnPresenterChanged()
        {
            serializedObject.ApplyModifiedProperties();
            _baseView.RebindPresenter();
        }

        protected virtual void DrawPresenterGUI()
        {
            EditorGUILayout.PropertyField(presenter_prop, new GUIContent("Presenter"));
        }

        protected virtual void DrawGUIItems()
        {
            EditorGUILayout.PropertyField(showAxes_prop, new GUIContent("Show Axes"));
            if (showAxes_prop.boolValue)
                EditorGUILayout.PropertyField(axisViewPrefab_prop, new GUIContent("AxisView Prefab"));
            EditorGUILayout.PropertyField(style_prop, new GUIContent("Visualization Style"));
            EditorGUILayout.PropertyField(size_prop, new GUIContent("Size"));
        }
    }
}
