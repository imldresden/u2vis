using u2vis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public class LegendController : MonoBehaviour
    {
        [SerializeField]
        private MultiDimDataPresenter _presenter;
        [SerializeField]
        private GenericVisualizationStyle _style;
        [SerializeField]
        private Vector3 _scale = Vector3.one;
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private float _width = 1.5f;
        [SerializeField]
        private GameObject _legendPrefab;

        [ContextMenu("RebuildLegend")]
        public void RebuildLegend()
        {
            GameObject legend = Instantiate(_legendPrefab, this.transform);
            LegendBuilder builder = legend.GetComponent<LegendBuilder>();
            builder.Presenter = _presenter;
            builder.Position = _position;
            builder.Width = _width;
            builder.Scale = _scale;
            builder.Style = _style;
            builder.RebuildLegend();
        }

    }
}