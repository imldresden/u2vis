using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(MultiDimDataPresenter))]
    public class MultiDimDataPresenterEditor : GenericDataPresenterEditor
    {
        protected SerializedProperty selectedIndexForXAxis_prop;
        protected MultiDimDataPresenter _gridPresenter = null;
        public override void OnEnable()
        {
            base.OnEnable();
            selectedIndexForXAxis_prop = serializedObject.FindProperty("_selectedIndexForXAxis");
            _gridPresenter = (MultiDimDataPresenter)_presenter;
        }

        protected override void DrawVisualizationDimensions()
        {
            DrawXDimensionGUI();
            EditorGUILayout.Separator();
            DrawYDimensionGUI();
            EditorGUILayout.Separator();
            DrawZDimensionGUI();
        }

        protected virtual void DrawXDimensionGUI()
        {
            int oldValue = selectedIndexForXAxis_prop.intValue;
            var axesPresenter = axisPresenters_prop.GetArrayElementAtIndex(0);
            EditorGUILayout.LabelField("X Axis", EditorStyles.boldLabel);
            int newSelection = EditorGUILayout.Popup("Caption Dimension", oldValue, _dimensionCaptions);
            if (oldValue != newSelection)
            {
                selectedIndexForXAxis_prop.intValue = newSelection;
                serializedObject.ApplyModifiedProperties();
                axesPresenter.FindPropertyRelative("_caption").stringValue = _dimensionCaptions[newSelection];
            }
            DrawAxisGUI(0);
        }

        protected virtual void DrawYDimensionGUI()
        {
            var axesPresenter = axisPresenters_prop.GetArrayElementAtIndex(1);
            EditorGUILayout.LabelField("Y Axis (Values)", EditorStyles.boldLabel);
            DrawAxisGUI(1);
        }

        protected virtual void DrawZDimensionGUI()
        {
            var axesPresenter = axisPresenters_prop.GetArrayElementAtIndex(2);
            EditorGUILayout.LabelField("Z Axis (Multiple Dimensions)", EditorStyles.boldLabel);
            DrawAxisGUI(2);
            DrawDimensionNumberSelection(false);
            for (int i = 0; i < dimensionsIndices_prop.arraySize; i++)
            {
                var oldIndex = dimensionsIndices_prop.GetArrayElementAtIndex(i).intValue;
                int newSelection = EditorGUILayout.Popup("Dimension", oldIndex, _dimensionCaptions);
                if (newSelection != oldIndex)
                {
                    dimensionsIndices_prop.GetArrayElementAtIndex(i).intValue = newSelection;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
