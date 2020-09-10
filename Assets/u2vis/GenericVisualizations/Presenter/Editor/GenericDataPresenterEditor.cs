using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(GenericDataPresenter))]
    public class GenericDataPresenterEditor : Editor
    {
        protected GenericDataPresenter _presenter = null;
        protected DataSet _data = null;
        protected string[] _dimensionCaptions = null;

        protected SerializedProperty
            dataProvider_prop,
            dimensions_prop,
            dimensionsIndices_prop,
            selectedMinItem_prop,
            selectedMaxItem_prop,
            axisPresenters_prop,
            highlightedItems_prop;

        public virtual void OnEnable()
        {
            dataProvider_prop = serializedObject.FindProperty("_dataProvider");
            dimensionsIndices_prop = serializedObject.FindProperty("_dataDimensionsIndices");
            selectedMinItem_prop = serializedObject.FindProperty("_selectedMinItem");
            selectedMaxItem_prop = serializedObject.FindProperty("_selectedMaxItem");
            axisPresenters_prop = serializedObject.FindProperty("_axisPresenters");
            highlightedItems_prop = serializedObject.FindProperty("_highlightedItems");

            _presenter = (GenericDataPresenter)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDataPresenterGUI();

            if (GUILayout.Button("reset Axes"))
                _presenter.ResetAxisProperties();

            if (_presenter.DataProvider != null)
            {
                SetData();
                SetDimensionCaptions();
                DrawGUIItems();
            }
            else
                EditorGUILayout.HelpBox("No DataProvider has been set!", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawDataPresenterGUI()
        {
            EditorGUILayout.PropertyField(dataProvider_prop, new GUIContent("Data Provider"));
        }

        protected virtual void SetData()
        {
            if (_presenter.DataProvider == null)
                return;
            _data = _presenter.DataProvider.Data;
            int count = _data.NumOfItems;
            if (selectedMinItem_prop.intValue > count)
                selectedMinItem_prop.intValue = count;
            if (selectedMaxItem_prop.intValue > count)
                selectedMaxItem_prop.intValue = count;
            highlightedItems_prop.arraySize = count;
        }

        protected virtual void SetDimensionCaptions()
        {
            if (_data == null)
                return;
            _dimensionCaptions = new string[_data.Count];
            for (int i = 0; i < _data.Count; i++)
                _dimensionCaptions[i] = _data[i].Name;
        }

        protected virtual void DrawGUIItems()
        {
            DrawIndexRangeSelectionGUI();
            DrawVisualizationDimensions();
        }

        protected virtual void DrawIndexRangeSelectionGUI()
        {
            var rightAligned = new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight };
            EditorGUILayout.LabelField("Item Index Range", EditorStyles.boldLabel);
            float minVal = _presenter.SelectedMinItem;
            float maxVal = _presenter.SelectedMaxItem;

            EditorGUILayout.BeginHorizontal();
            minVal = EditorGUILayout.IntField((int)minVal);
            maxVal = EditorGUILayout.IntField((int)maxVal, rightAligned);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, 0, _data.NumOfItems);
            if (minVal != _presenter.SelectedMinItem)
                _presenter.SelectedMinItem = (int)minVal;
            if (maxVal != _presenter.SelectedMaxItem)
                _presenter.SelectedMaxItem = (int)maxVal;
        }

        protected virtual void DrawVisualizationDimensions()
        {
            DrawDimensionNumberSelection();
            EditorGUILayout.Separator();
            // Draw each individual dimension
            for (int i = 0; i < dimensionsIndices_prop.arraySize; i++)
            {
                DrawSingleDimensionGUI(i);
                EditorGUILayout.Separator();
            }
        }

        protected virtual void DrawDimensionNumberSelection(bool alsoResizeAxisArray = true)
        {
            EditorGUILayout.LabelField("Number of Dimensions", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            int dimensionIndexSize = dimensionsIndices_prop.arraySize;
            if (GUILayout.Button("-"))
                dimensionIndexSize--;
            dimensionIndexSize = EditorGUILayout.IntField(dimensionIndexSize);
            if (GUILayout.Button("+"))
                dimensionIndexSize++;
            ResizeDimensionIndexArray(dimensionIndexSize);
            if (alsoResizeAxisArray)
                ResizeAxisPresenterArray(dimensionIndexSize);
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawSingleDimensionGUI(int dimensionsIndex)
        {
            var dataIndex = dimensionsIndices_prop.GetArrayElementAtIndex(dimensionsIndex).intValue;
            var axesPresenter = axisPresenters_prop.GetArrayElementAtIndex(dimensionsIndex);
            EditorGUILayout.LabelField("Axis #" + dimensionsIndex, EditorStyles.boldLabel);
            int newSelection = EditorGUILayout.Popup("Dimension", dataIndex, _dimensionCaptions);
            if (newSelection != dataIndex)
            {
                dimensionsIndices_prop.GetArrayElementAtIndex(dimensionsIndex).intValue = newSelection;
                serializedObject.ApplyModifiedProperties();
                axesPresenter.FindPropertyRelative("_caption").stringValue = _dimensionCaptions[newSelection];
            }
            DrawAxisGUI(dimensionsIndex);
        }

        protected virtual void DrawAxisGUI(int axisIndex)
        {
            var axesPresenter = axisPresenters_prop.GetArrayElementAtIndex(axisIndex);
            var isCategorical = axesPresenter.FindPropertyRelative("_isCategorical");
            isCategorical.boolValue = EditorGUILayout.Toggle(new GUIContent("Is Categorical?", "Check this if you want the axis to be treated as categorical even when the data is numerical in nature."), isCategorical.boolValue);
            if (!isCategorical.boolValue)
            {
                EditorGUILayout.PropertyField(axesPresenter.FindPropertyRelative("_tickIntervall"), new GUIContent("Tick Intervall", "Between 0 and 1, determines on which distances there will be ticks on the axis."));
                EditorGUILayout.PropertyField(axesPresenter.FindPropertyRelative("_decimalPlaces"), new GUIContent("Decimal Places", "Determines the number of digits for numerical values."));
            }
            EditorGUILayout.PropertyField(axesPresenter.FindPropertyRelative("_labelTickIntervall"), new GUIContent("Label Tick Intervall", "Determines which of the visible ticks will also have a label. 1 means every tick, 2 every second tick and so on."));
            EditorGUILayout.PropertyField(axesPresenter.FindPropertyRelative("_labelOrientation"), new GUIContent("Label Orientation"));
        }

        protected virtual void ResizeDimensionIndexArray(int size)
        {
            if (size < 1)
                size = 1;
            int length = dimensionsIndices_prop.arraySize;
            if (size == length)
                return;
            dimensionsIndices_prop.arraySize = size;
            serializedObject.ApplyModifiedProperties();
            for (int i = length; i < size; i++)
                dimensionsIndices_prop.GetArrayElementAtIndex(i).intValue = GenericDataPresenter.DEFAULT_INDEX;
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void ResizeAxisPresenterArray(int size)
        {
            axisPresenters_prop.arraySize = size;
        }
    }
}
