using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public class LegendBuilder : MonoBehaviour
    {
        [SerializeField]
        private GenericDataPresenter _presenter;
        [SerializeField]
        private GenericVisualizationStyle _style;
        [SerializeField]
        private bool _useBackground = true;
        [SerializeField]
        private Vector3 _scale = Vector3.one;
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private float _width = 1.5f;
        [SerializeField]
        private RectTransform _background;
        [SerializeField]
        private RectTransform _legendLabel;
        [SerializeField]
        private GameObject _labelsParent;
        [SerializeField]
        private GameObject _labelPrefab;
        [SerializeField]
        private bool _useValues = false;
        [SerializeField]
        private int _valueDim = 0;

        public GenericDataPresenter Presenter { get => _presenter; set => _presenter = value; }
        public GenericVisualizationStyle Style { get => _style; set => _style = value; }
        
        public Vector3 Scale { get => _scale; set => _scale = value; }
        
        public Vector3 Position { get => _position; set => _position = value; }
        public float Width { get => _width; set => _width = value; }

        void Start()
        {
            //RebuildLegend();
        }

        // Update is called once per frame
        void Update()
        {

        }

        [ContextMenu("RebuildLegend")]
        public void RebuildLegend()
        {

            for (int i = _labelsParent.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(_labelsParent.transform.GetChild(i).gameObject);
            _legendLabel.sizeDelta = new Vector2(_width/0.001f,_legendLabel.sizeDelta.y);
            _background.sizeDelta = (new Vector2(_width/0.001f, _legendLabel.sizeDelta.y));
            _legendLabel.gameObject.transform.parent.localPosition = _position;
            _background.localPosition = new Vector3(_position.x,_position.y + (_legendLabel.sizeDelta.y*_legendLabel.localScale.y),_position.z + 0.0001f);

            if (_useValues)
                for (int i = 0; i < _presenter.SelectedItemsCount; i++)
                    BuildLabel(i, _presenter.DataProvider.Data[_valueDim].GetObjValue(i + _presenter.SelectedMinItem).ToString(), _width);
            else
                for (int i = 0; i < _presenter.NumberOfDimensions; i++)
                    BuildLabel(i, _presenter[i].Name, _width);
            this.gameObject.transform.localScale = _scale;
            _background.gameObject.SetActive(_useBackground);
        }

        public void BuildLabel(int index, string labelText, float width)
        {
            GameObject label = Instantiate(_labelPrefab, _labelsParent.transform);
            LabelController labelController = label.GetComponent<LabelController>();
            label.transform.localPosition = new Vector3(_position.x, _position.y - (index * labelController.GetHeight()), _position.z);
            labelController.SetLabelColor(_style.GetColorCategorical(index));
            labelController.SetLabelText(labelText);
            labelController.SetWidth(width);
            _background.sizeDelta = (new Vector2(width/0.001f, _background.sizeDelta.y + labelController.GetHeight() / 0.001f));
        }
    }
}
