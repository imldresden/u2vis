using UnityEditor;
using UnityEngine;

namespace u2vis.InfoVis
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
