using UnityEditor;
using UnityEngine;

namespace UVis.InfoVis
{
    [CustomEditor(typeof(Heightmap))]
    public class HeightmapEditor : BaseVisualizationViewEditor
    {
        protected Heightmap _heightmap = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            _heightmap = (Heightmap)serializedObject.targetObject;
        }
    }
}
