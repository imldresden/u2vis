using System;
using System.Collections.Generic;
using UnityEngine;
using u2vis.Utilities;

namespace u2vis
{
    public class StackedBar : BaseVisualizationView,IStackedVis
    {
        [SerializeField]
        private Mesh _dataItemMesh = null;
        [SerializeField]
        private int _valueIndex = 0;
        [SerializeField]
        private int _segments = 360;
        [SerializeField]
        private bool _useMinIndex = false;

        private float _divisor;
        public Mesh DataItemMesh
        {
            get { return _dataItemMesh; }
            set
            {
                _dataItemMesh = value;
                Rebuild();
            }
        }

        public int ValueIndex
        {
            get { return _valueIndex; }
            set
            {
                _valueIndex = value;
                Rebuild();
            }
        }

        #region Constructors
        protected StackedBar() : base()
        {
        }
        #endregion

        #region Protected Methods
        protected override void RebuildVisualization()
        {
            if (_presenter == null || _presenter.NumberOfDimensions < 3)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            if (_dataItemMesh == null)
            {
                _dataItemMesh = buildCircleMesh();
                Debug.Log("No DataMesh was set for this visualization. Using default");
            }
            var iMesh = new IntermediateMesh();
            // temporary save the mesh data from the template for faster access
            var tVertices = _dataItemMesh.vertices;
            var tNromals = _dataItemMesh.normals;
            var tUVs = _dataItemMesh.uv;
            var tIndices = _dataItemMesh.triangles;

            //int offset = _presenter.SelectedMinItem;
            float divisor = 0;

            MultiDimDataPresenter presenter = (MultiDimDataPresenter)_presenter;
            //TODO find better way to determine divisor (what 100% is)
            for (int valueIndex = 0; valueIndex < presenter[1].Count; valueIndex++)
            {
                float sum = 0;
                for (int dimIndex = 0; dimIndex < presenter.NumberOfDimensions; dimIndex++)
                {
                    sum += VisViewHelper.GetItemValueAbsolute(presenter, dimIndex, valueIndex);
                }
                divisor = Mathf.Max(divisor, sum);
                //Debug.Log("Div " + divisor);
            }
            float startHeight = 0;
            _divisor = divisor;
            //Debug.Log(presenter.CaptionDimension.GetStringValue(ValueIndex));

            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                float dim;
                if (_useMinIndex)
                {
                    dim = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, _presenter.SelectedMinItem);
                }
                else
                {
                    dim = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, _valueIndex);
                }
                float height = dim / divisor;
                //Debug.Log(presenter[dimIndex].Name + " height " + height+ " dim " + dim +" div " + divisor);
                //var pos = new Vector3(0, startHeight * _size.y, 0);
                var scale = new Vector3(_size.x, height * _size.y, _size.z);
                var startIndex = iMesh.Vertices.Count;
                foreach (var v in tVertices)
                {
                    var vPos = new Vector3(v.x * scale.x, startHeight * _size.y + v.y * scale.y, v.z * scale.z);
                    iMesh.Vertices.Add(vPos);
                    iMesh.Colors.Add(_style.GetColorCategorical(dimIndex, height));
                }
                iMesh.Normals.AddRange(tNromals);
                iMesh.TexCoords.AddRange(tUVs);
                foreach (var index in tIndices)
                    iMesh.Indices.Add(startIndex + index);
                startHeight += height;
            }

            var mesh = iMesh.GenerateMesh("StackedBarMesh", MeshTopology.Triangles);
            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
                meshCollider.sharedMesh = mesh;
        }

        private Mesh buildCircleMesh()
        {
            Mesh mesh = new Mesh();
            var vertices = new List<Vector3>();
            var colors = new List<Color>();
            var indices = new List<int>();

            List<Vector3> partMeshTop = Circles.CreatePartMesh(0, 2 * Mathf.PI, 0.5f, _segments);
            for (int vertex = 0; vertex < partMeshTop.Count; vertex++)
            {
                partMeshTop[vertex] = new Vector3(partMeshTop[vertex].x, 1,partMeshTop[vertex].y);
            }
            vertices.AddRange(partMeshTop);
            List<Vector3> partMeshBottom = Circles.CreatePartMesh(0, 2 * Mathf.PI, 0.5f, _segments);
            for (int vertex = 0; vertex < partMeshBottom.Count; vertex++)
            {
                partMeshBottom[vertex] = new Vector3(partMeshBottom[vertex].x, 0,partMeshBottom[vertex].y);
            }
            vertices.AddRange(partMeshBottom);
            List<int> round = Circles.CreateRound(vertices.Count - partMeshBottom.Count - partMeshTop.Count, partMeshTop.Count -1,0);
            indices.AddRange(round);

            vertices.Add(new Vector3(0, 1, 0));
            List<Vector3> partMeshTopPlate = Circles.CreatePartMesh(0, 2 * Mathf.PI, 0.5f, _segments);
            partMeshTopPlate[0] = new Vector3(partMeshTopPlate[0].x,1, partMeshTopPlate[0].y);
            for (int vertex = 1; vertex < partMeshTopPlate.Count; vertex++)
            {
                partMeshTopPlate[vertex] = new Vector3(partMeshTopPlate[vertex].x,1, partMeshTopPlate[vertex].y);
                indices.Add(vertices.Count - 1);
                indices.Add(vertices.Count + vertex);
                indices.Add(vertices.Count - 1 + vertex);
            }
            vertices.AddRange(partMeshTopPlate);

            mesh.name = "CylinderTopPlate";
            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
        #endregion

        protected override void SetupInitialAxisViews()
        {
            DestroyAxisViews();
            _axisViews = new List<GenericAxisView>();
            // Generic X Axis
            var vX = Instantiate(_axisViewPrefab, transform, false);
            vX.AxisPresenter = _presenter.AxisPresenters[0];
            var segmentStartList = GetSegmentStartList(new Vector3(1f, 0f, 0f));
            vX.Length = segmentStartList[segmentStartList.Count - 1].x;
            vX.transform.localRotation = Quaternion.Euler(0, 90, 90);
            vX.transform.localPosition = new Vector3(vX.transform.localPosition.x, vX.transform.localPosition.y, vX.transform.localPosition.z - _size.z/1.8f);
            _axisViews.Add(vX);
        }
        protected override void RebuildAxes()
        {
            if (_axisViews == null || _fromEditor)
                SetupInitialAxisViews();
            List<AxisTick> ticks = new List<AxisTick>();
            var segmentStartList = GetSegmentStartList(new Vector3(1f, 0f, 0f));
            ticks.Add(new AxisTick(0,"0"));
            float oldValue = 0;
            for (int index = 1; index <segmentStartList.Count; index++)
            {

                if (_useMinIndex)
                {
                    float newValue = oldValue + VisViewHelper.GetItemValueAbsolute(_presenter, index - 1, _presenter.SelectedMinItem);
                    ticks.Add(new AxisTick(segmentStartList[index].x / _axisViews[0].Length, newValue.ToString()));
                    oldValue = newValue;
                }
                else
                {
                    float newValue = oldValue + VisViewHelper.GetItemValueAbsolute(_presenter, index - 1, _valueIndex);
                    ticks.Add(new AxisTick(segmentStartList[index].x / _axisViews[0].Length, newValue.ToString()));
                    oldValue = newValue;
                }
            }
            //TODO: Find way to determine name of axis
                _axisViews[0].RebuildAxis(ticks.ToArray());

        }

        public virtual void Initialize(GenericDataPresenter presenter, GenericAxisView axisViewPrefab = null, GenericVisualizationStyle style = null, Mesh dataItemMesh = null)
        {
            _dataItemMesh = dataItemMesh;
            base.Initialize(presenter, axisViewPrefab, style);
        }

        public List<Vector3> GetSegmentStartList(Vector3 normDirectionVector)
        {
            List<Vector3> heights = new List<Vector3>();
            Vector3 locSc = this.gameObject.transform.localScale;
            float startHeight = 0;
            heights.Add(new Vector3(0,0,0));
            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                float dim;
                if (_useMinIndex)
                {
                    dim = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, _presenter.SelectedMinItem);
                }
                else
                {
                    dim = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, _valueIndex);
                }
                float height = (dim / _divisor)*_size.y;
                startHeight += height;
                Vector3 heightVec = new Vector3(locSc.x * normDirectionVector.x * startHeight,locSc.y * normDirectionVector.y * startHeight, locSc.z * normDirectionVector.z * startHeight);
                heights.Add(heightVec);
            }
            return heights;
        }
    }
}
