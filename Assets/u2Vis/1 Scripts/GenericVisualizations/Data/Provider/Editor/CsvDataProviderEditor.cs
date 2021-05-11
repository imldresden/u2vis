using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
{
    [CustomEditor(typeof(CsvDataProvider))]
    public class CsvDataProviderEditor : Editor
    {
        SerializedProperty csvFile_prop,
            seperator_prop,
            rowBasedLayout_prop,
            seconFieldContainesDataType_prop;
        CsvDataProvider _provider = null;

        private void OnEnable()
        {
            csvFile_prop = serializedObject.FindProperty("_csvFile");
            seperator_prop = serializedObject.FindProperty("_seperator");
            rowBasedLayout_prop = serializedObject.FindProperty("_rowBasedLayout");
            seconFieldContainesDataType_prop = serializedObject.FindProperty("_seconFieldContainesDataType");

            _provider = (CsvDataProvider)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(csvFile_prop, new GUIContent("CSV File"));
            EditorGUILayout.PropertyField(seperator_prop, new GUIContent("Seperator"));
            EditorGUILayout.PropertyField(rowBasedLayout_prop, new GUIContent("Row-based Layout"));
            EditorGUILayout.PropertyField(seconFieldContainesDataType_prop, new GUIContent("Second row contains DataType"));
            if (GUILayout.Button("(Re)load Data"))
                _provider.LoadData();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
