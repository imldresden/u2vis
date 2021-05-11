using System.Collections.Generic;
using UnityEngine;
using u2vis.Utilities;

namespace u2vis
{
    public class ParallelCoordinates : BaseVisualizationView
    {
        #region Protected Methods
        protected override void SetupInitialAxisViews()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                if (transform.GetChild(i).GetComponent<GenericAxisView>() != null)
                    DestroyImmediate(transform.GetChild(i).gameObject);

            var length = _presenter.AxisPresenters.Length;
            _axisViews = new List<GenericAxisView>(length);
            for (int i = 0; i < length; i++)
            {
                float posX = (float)i / _presenter.NumberOfDimensions * _size.x;
                var axisView = Instantiate(_axisViewPrefab, transform, false);
                axisView.AxisPresenter = _presenter.AxisPresenters[i];
                axisView.Length = _size.y;
                axisView.transform.localRotation = Quaternion.Euler(0, 0, 90);
                axisView.transform.localPosition = new Vector3(posX, 0, 0);
                _axisViews.Add(axisView);
            }
        }

        protected override void RebuildVisualization()
        {
            if (_presenter == null || _presenter.NumberOfDimensions < 2)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            var iMesh = new IntermediateMesh();
            int offset = (int)_presenter.SelectedMinItem;
            int length = (int)_presenter.SelectedMaxItem - offset;
            for (int itemIndex = 0; itemIndex < length; itemIndex++)
            {
                for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
                {
                    float pos_x = (float)dimIndex / _presenter.NumberOfDimensions;
                    //Debug.Log(dimIndex + ", " + (itemIndex + offset));
                    float pos_y = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, itemIndex + offset, true);
                    iMesh.Vertices.Add(new Vector3(pos_x * _size.x, pos_y * _size.y, 0.0f));
                    iMesh.Normals.Add(-Vector3.forward);
                    iMesh.Colors.Add(_style.GetColorCategorical(itemIndex, pos_x));
                    if (dimIndex == 0)
                        continue;
                    iMesh.Indices.Add(iMesh.Vertices.Count - 2);
                    iMesh.Indices.Add(iMesh.Vertices.Count - 1);
                }
            }
            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = iMesh.GenerateMesh("ParallelCoordinatesMesh", MeshTopology.Lines);
        }
        #endregion
    }
}
