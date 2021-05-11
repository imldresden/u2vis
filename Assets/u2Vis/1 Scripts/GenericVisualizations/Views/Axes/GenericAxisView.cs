using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using u2vis.Utilities;

namespace u2vis
{
    /// <summary>
    /// Class that represents a view for genric axis for visualizations.
    /// </summary>
    public class GenericAxisView : MonoBehaviour
    {
        #region Private Fields
        /// <summary>
        /// The text prefab that is used for labels of axis ticks.
        /// </summary>
        [SerializeField]
        private Text _tickLabelPrefab = null;
        /// <summary>
        /// The text prefab that us used for the label of the axis itself.
        /// </summary>
        [SerializeField]
        private Text _axisLabelPrefab = null;
        /// <summary>
        /// Transform that represents the coordinate system root of the axis.
        /// </summary>
        [SerializeField]
        private Transform _axisRoot = null;
        /// <summary>
        /// Cavas that is used to hold all labels of the axis.
        /// </summary>
        [SerializeField]
        private Canvas _labelCanvas = null;
        /// <summary>
        /// The length of the tick line, which extends orthogonally from the axis.
        /// </summary>
        [SerializeField]
        private float _tickLength = 0.02f;
        /// <summary>
        /// The offset of the label from the end of the tick line.
        /// </summary>
        [SerializeField]
        private float _labelOffset = 0.01f;

        /// <summary>
        /// The axis presenter which this axis view is based on.
        /// </summary>
        [SerializeField]
        private AxisPresenter _axisPresenter = null;
        /// <summary>
        /// The length of the axis in unity units.
        /// </summary>
        [SerializeField]
        private float _length = 1.0f;
        /// <summary>
        /// Determines if the side of the axis the labels are shown is swapped or not.
        /// For a vertical axis, swapped shows labels on the right, not swapped on the left.
        /// </summary>
        [SerializeField]
        private bool _swapped = false;
        /// <summary>
        /// Determines if the ticks on the axis are mirrored, i.e., if they start at the beginning or the end of the axis.
        /// </summary>
        [SerializeField]
        private bool _mirrored = false;
        /// <summary>
        /// Determines if a label is shown for the whole axis.
        /// </summary>
        [SerializeField]
        private bool _hasAxisLabel = false;
        /// <summary>
        /// The offset to the axis root at which the axis label is shown.
        /// </summary>
        [SerializeField]
        private float _axisLabelOffset= 0.1f;
        /// <summary>
        /// Inverse scale vector of the canvas that hosts the labels. used to determine the correct position of the labels.
        /// </summary>
        private Vector3 _invCanvasScale = Vector3.one;
        #endregion

        #region Public Properties
        /// <summary>
        /// The axis presenter this axis view is based on.
        /// </summary>
        public AxisPresenter AxisPresenter
        {
            get { return _axisPresenter; }
            set { _axisPresenter = value; }
        }
        /// <summary>
        /// The length of the axis in unity units.
        /// </summary>
        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }
        /// <summary>
        /// Determines if the side of the axis the labels are shown is swapped or not.
        /// For a vertical axis, swapped shows labels on the right, not swapped on the left.
        /// </summary>
        public bool Swapped
        {
            get { return _swapped; }
            set { _swapped = value; }
        }
        /// <summary>
        /// Determines if the ticks on the axis are mirrored, i.e., if they start at the beginning or the end of the axis.
        /// </summary>
        public bool Mirrored
        {
            get { return _mirrored; }
            set { _mirrored = value; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Called by unity once at the start of the script.
        /// </summary>
        protected virtual void Start()
        {
        }
        /// <summary>
        /// Creates the label for the whole axis based on the provided caption.
        /// </summary>
        /// <param name="axisLabel">The caption for the axis label.</param>
        protected void CreateAxisLabel(string axisLabel)
        {
            var label = Instantiate(_axisLabelPrefab, _labelCanvas.transform, false);
            label.text = axisLabel;
            var rectTrans = label.GetComponent<RectTransform>();
            float posX = _length / 2;
            float posY = (_axisLabelOffset);
            if (!_swapped)
                posY *= -1;
            rectTrans.localPosition = new Vector3(posX * _invCanvasScale.x, posY * _invCanvasScale.y, 0.0f);
            rectTrans.localRotation = Quaternion.Euler(0, 0, 0);
        }
        /// <summary>
        /// Creates a single tick line for the axis.
        /// </summary>
        /// <param name="iMesh">The intermediate mesh holding the components for the final mesh.</param>
        /// <param name="tick">The tick data.</param>
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
        /// <summary>
        /// Creates the label for a single tick.
        /// </summary>
        /// <param name="tick">the tick data.</param>
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
        #endregion

        #region Public Methods
        /// <summary>
        /// Rebuilds the axis based on the specified list of ticks.
        /// </summary>
        /// <param name="ticks">The list of ticks representing the topology of the axis.</param>
        /// <param name="axisLabel">The label shown for the whole axis.</param>
        public void RebuildAxis(AxisTick[] ticks, string axisLabel = null)
        {
            var meshFilter = GetComponent<MeshFilter>();

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

            GameObject.Destroy(meshFilter.sharedMesh);
            meshFilter.sharedMesh = iMesh.GenerateMesh("AxisTicksMesh", MeshTopology.Lines);
        }
        #endregion
    }
}
