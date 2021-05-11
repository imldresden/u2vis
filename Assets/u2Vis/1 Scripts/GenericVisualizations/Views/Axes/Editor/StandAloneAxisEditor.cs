using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(StandAloneAxis))]
    public class StandAloneAxisEditor : Editor
    {
        protected SerializedProperty
            presenter_prop,
            axisPresenterIndex_prop,
            axisViewPrefab_prop,
            axisLength_prop,
            swapped_prop,
            mirrored_prop,
            generationMethod_prop,
            modeParam1_prop,
            modeParam2_prop,
            min_prop,
            max_prop;

        StandAloneAxis _standAloneAxis = null;
        string[] _axisPresenterCaptions = null;
        string[] _dimensionCaptions = null;

        protected virtual void OnEnable()
        {
            presenter_prop = serializedObject.FindProperty("_presenter");
            axisPresenterIndex_prop = serializedObject.FindProperty("_axisPresenterIndex");
            axisViewPrefab_prop = serializedObject.FindProperty("_axisViewPrefab");
            axisLength_prop = serializedObject.FindProperty("_axisLength");
            swapped_prop = serializedObject.FindProperty("_swapped");
            mirrored_prop = serializedObject.FindProperty("_mirrored");
            generationMethod_prop = serializedObject.FindProperty("_generationMethod");
            modeParam1_prop = serializedObject.FindProperty("_modeParam1");
            modeParam2_prop = serializedObject.FindProperty("_modeParam2");
            min_prop = serializedObject.FindProperty("_min");
            max_prop = serializedObject.FindProperty("_max");

            _standAloneAxis = (StandAloneAxis)serializedObject.targetObject;
            if (_standAloneAxis == null)
                return;
            GenerateAxisPresenterCaptions();
            GenerateDimensionCaptions();
        }
        protected virtual void GenerateAxisPresenterCaptions()
        {
            _axisPresenterCaptions = null;
            if (_standAloneAxis.DataPresenter == null)
                return;
            var axisPresenters = _standAloneAxis.DataPresenter.AxisPresenters;
            _axisPresenterCaptions = new string[axisPresenters.Length];
            for (int i = 0; i < axisPresenters.Length; i++)
                _axisPresenterCaptions[i] = axisPresenters[i].Caption;
        }

        protected virtual void GenerateDimensionCaptions()
        {
            _dimensionCaptions = null;
            if (_standAloneAxis.DataPresenter == null)
                return;
            var length = _standAloneAxis.DataPresenter.NumberOfDimensions;
            _dimensionCaptions = new string[length];
            for (int i = 0; i < length; i++)
                _dimensionCaptions[i] = _standAloneAxis.DataPresenter[i].Name;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(axisViewPrefab_prop, new GUIContent("Axis Prefab"));
            if (_standAloneAxis.AxisViewPrefab == null)
                EditorGUILayout.HelpBox("No AxisViewPrefab set!", MessageType.Warning);

            EditorGUILayout.PropertyField(presenter_prop, new GUIContent("Data Presenter"));
            if (_standAloneAxis.DataPresenter != null)
            {
                DrawAxisPresenterGUI();
                EditorGUILayout.PropertyField(axisLength_prop, new GUIContent("Axis Length"));
                EditorGUILayout.PropertyField(swapped_prop, new GUIContent("Swapp Labels"));
                EditorGUILayout.PropertyField(mirrored_prop, new GUIContent("Mirror Axis"));
                DrawGenerationModeGUI();
            }
            else
                EditorGUILayout.HelpBox("No DataPresenter set!", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
            if (_standAloneAxis.DataPresenter != null && _standAloneAxis.AxisViewPrefab != null)
                if (GUILayout.Button("Rebuild Axis"))
                    _standAloneAxis.RebuildAxis();
        }


        protected virtual void DrawAxisPresenterGUI()
        {
            if (_axisPresenterCaptions != null)
                axisPresenterIndex_prop.intValue = EditorGUILayout.Popup(new GUIContent("Axis Presenter"), axisPresenterIndex_prop.intValue, _axisPresenterCaptions);
        }

        protected virtual void DrawGenerationModeGUI()
        {
            EditorGUILayout.PropertyField(generationMethod_prop, new GUIContent("Generation Method"));
            switch ((StandAloneAxis.GenerationMethod)generationMethod_prop.enumValueIndex)
            {
                case StandAloneAxis.GenerationMethod.Dimension:
                    modeParam1_prop.intValue = EditorGUILayout.Popup(new GUIContent("DataDimension"), modeParam1_prop.intValue, _dimensionCaptions);
                    break;
                case StandAloneAxis.GenerationMethod.DimensionCaptions:
                    modeParam1_prop.intValue = EditorGUILayout.Popup(new GUIContent("Start Dimension"), modeParam1_prop.intValue, _dimensionCaptions);
                    modeParam2_prop.intValue = EditorGUILayout.Popup(new GUIContent("End Dimension"), modeParam2_prop.intValue, _dimensionCaptions);
                    break;
                case StandAloneAxis.GenerationMethod.DiscreteRange:
                    EditorGUILayout.PropertyField(modeParam1_prop, new GUIContent("Range Start"));
                    EditorGUILayout.PropertyField(modeParam2_prop, new GUIContent("Range End"));
                    break;
                case StandAloneAxis.GenerationMethod.MinMaxValue:
                    EditorGUILayout.PropertyField(min_prop, new GUIContent("Minmum Value"));
                    EditorGUILayout.PropertyField(max_prop, new GUIContent("Maximum Value"));
                    break;
            }
        }
    }
}
