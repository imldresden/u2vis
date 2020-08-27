using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UVis.InfoVis
{
    [CustomEditor(typeof(RectGrid))]
    public class GridEditor : Editor
    {
        SerializedProperty size_prop, spacing_prop;
        RectGrid _grid = null;

        private void OnEnable()
        {
            size_prop = serializedObject.FindProperty("_size");
            spacing_prop = serializedObject.FindProperty("_spacing");
            _grid = (RectGrid)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(size_prop, new GUIContent("Size"));
            EditorGUILayout.PropertyField(spacing_prop, new GUIContent("Spacing"));
            if (GUILayout.Button("Rebuild Grid"))
                _grid.RebuildGridGeometry();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
