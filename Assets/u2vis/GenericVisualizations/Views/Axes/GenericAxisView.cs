using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using u2vis.Utilities;

namespace u2vis
{
    public class GenericAxisView : MonoBehaviour
    {
        [SerializeField]
        private Text _tickLabelPrefab = null;
        [SerializeField]
        private Text _axisLabelPrefab = null;
        [SerializeField]
        private Transform _axisRoot = null;
        [SerializeField]
        private Canvas _labelCanvas = null;
        [SerializeField]
        private float _tickLength = 0.02f;
        [SerializeField]
        private float _labelOffset = 0.01f;

        [SerializeField]
        private AxisPresenter _axisPresenter = null;
        [SerializeField]
        private float _length = 1.0f;
        [SerializeField]
        private bool _swapped = false;
        [SerializeField]
        private bool _mirrored = false;
        [SerializeField]
        private bool _hasAxisLabel = false;
        [SerializeField]
        private float _axisLabelOffset= 0.1f;
        private Vector3 _invCanvasScale = Vector3.one;

        private MeshFilter _meshFilter;

        #region Public Properties
        public AxisPresenter AxisPresenter
        {
            get { return _axisPresenter; }
            set { _axisPresenter = value; }
        }

        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public bool Swapped
        {
            get { return _swapped; }
            set { _swapped = value; }
        }

        public bool Mirrored
        {
            get { return _mirrored; }
            set { _mirrored = value; }
        }

        #endregion

        protected virtual void Start()
        {
        }

        public void RebuildAxis(AxisTick[] ticks, string axisLabel = null)
        {
            if (_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();

            var canvasScale = _labelCanvas.transform.localScale;
            _invCanvasScale = new Vector3(1.0f / canvasScale.x, 1.0f / canvasScale.y, 1.0f / canvasScale.z);

            _axisRoot.localScale = new Vector3(_length, 1, 1);
            // set the canvas to the same size as the axis. Technically, there is no need to, but it looks nicer in the editor.
            _labelCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _length * _invCanvasScale.x);
            // Remove all previous labels
            for (int i = _labelCanvas.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(_labelCanvas.transform.GetChild(i).gameObject);
            // Create new Ticks & Labels
            var iMesh = new IntermediateMesh();
            for (int i = 0; i < ticks.Length; i++)
            {
                CreateSingleTick(iMesh, ticks[i]);
                if (ticks[i].HasLabel)
                    CreateSingleLabel(ticks[i]);
            }
            if (_hasAxisLabel)
                CreateAxisLabel(axisLabel);

            GameObject.Destroy(_meshFilter.sharedMesh);
            _meshFilter.sharedMesh = iMesh.GenerateMesh("AxisTicksMesh", MeshTopology.Lines);
        }

        private void CreateAxisLabel(string axisLabel)
        {
            var label = Instantiate(_axisLabelPrefab, _labelCanvas.transform, false);
            label.text = axisLabel;
            var rectTrans = label.GetComponent<RectTransform>();
            float posX = _length/2;
            float posY = (_axisLabelOffset);
            if (!_swapped)
                posY *= -1;
            rectTrans.localPosition = new Vector3(posX * _invCanvasScale.x, posY * _invCanvasScale.y, 0.0f);
            rectTrans.localRotation = Quaternion.Euler(0, 0, 0);
        }

        protected void CreateSingleTick(IntermediateMesh iMesh, AxisTick tick)
        {
            float posX = tick.Position * _length;
            if (_mirrored)
                posX = _length - posX;
            float posY = _tickLength;
            if (!_swapped)
                posY *= -1;

            iMesh.Vertices.Add(new Vector3(posX, 0, 0));
            iMesh.Vertices.Add(new Vector3(posX, posY, 0));
            iMesh.Normals.Add(-Vector3.forward);
            iMesh.Normals.Add(-Vector3.forward);
            iMesh.Colors.Add(Color.white);
            iMesh.Colors.Add(Color.white);
            iMesh.Indices.Add(iMesh.Vertices.Count - 2);
            iMesh.Indices.Add(iMesh.Vertices.Count - 1);
        }

        protected void CreateSingleLabel(AxisTick tick)
        {
            var label = Instantiate(_tickLabelPrefab, _labelCanvas.transform, false);
            label.text = tick.Label;
            var rectTrans = label.GetComponent<RectTransform>();
            if (_axisPresenter.LabelOrientation == LabelOrientation.Parallel)
            {
                rectTrans.pivot = new Vector2(0.5f, _swapped ? 0.0f : 1.0f);
                label.alignment = TextAnchor.MiddleCenter;
            }
            else
            {
                rectTrans.pivot = new Vector2(_swapped ? 1.0f : 0.0f, 0.5f);
                label.alignment = _swapped ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
            }

            float posX = tick.Position * _length;
            if (_mirrored)
                posX = _length - posX;
            float posY = (_tickLength + _labelOffset);
            if (!_swapped)
                posY *= -1;
            rectTrans.localPosition = new Vector3(posX * _invCanvasScale.x, posY * _invCanvasScale.y, 0.0f);
            rectTrans.localRotation = Quaternion.Euler(0, 0, -45 * (int)_axisPresenter.LabelOrientation);
        }
    }
}
