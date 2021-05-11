using UnityEngine;
using u2vis.Utilities;

namespace u2vis
{
    public class Heightmap : BaseVisualizationView
    {
        #region Protected Methods

        protected override void SetupInitialAxisViews()
        {
            if (_presenter.AxisPresenters.Length < 3)
            {
                Debug.LogError("Data Presenter has fewer than three AxisPresenters");
                return;
            }
            base.SetupInitialAxisViews();
            var vZ = Instantiate(_axisViewPrefab, transform, false);
            vZ.AxisPresenter = _presenter.AxisPresenters[2];
            vZ.Length = _size.z;
            vZ.transform.localRotation = Quaternion.Euler(0, 90, 0);
            vZ.transform.localPosition = new Vector3(0, 0, _size.z);
            vZ.Mirrored = true;
            _axisViews.Add(vZ);
        }

        protected override void RebuildVisualization()
        {
            if (_presenter == null || _presenter.NumberOfDimensions < 3)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            var iMesh = new IntermediateMesh();
            int offset = _presenter.SelectedMinItem;
            int length = _presenter.SelectedItemsCount;
            float posXStep = _size.x / length;
            float posZStep = _size.z / _presenter.NumberOfDimensions;
            float maxValue = VisViewHelper.GetGlobalMaximum(_presenter);
            float uStep = 1.0f / length;
            float vStep = 1.0f / _presenter.NumberOfDimensions;
            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                for (int itemIndex = 0; itemIndex < length; itemIndex++)
                {
                    float value = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, itemIndex + offset) / maxValue;
                    var pos = new Vector3(posXStep * itemIndex, value, posZStep * dimIndex);
                    iMesh.Vertices.Add(new Vector3(pos.x * _size.x, pos.y * _size.y, pos.z * _size.z));
                    iMesh.Colors.Add(_style.GetColorContinous(uStep * itemIndex, vStep * dimIndex));
                    if (itemIndex < 1 || dimIndex < 1)
                        continue;
                    iMesh.Indices.Add(dimIndex * length + itemIndex);
                    iMesh.Indices.Add((dimIndex - 1) * length + itemIndex - 1);
                    iMesh.Indices.Add(dimIndex * length + itemIndex - 1);

                    iMesh.Indices.Add(dimIndex * length + itemIndex);
                    iMesh.Indices.Add((dimIndex - 1) * length + itemIndex);
                    iMesh.Indices.Add((dimIndex - 1) * length + itemIndex - 1);
                }
            }
            var mesh = iMesh.GenerateMesh("HeightmapMesh", MeshTopology.Triangles);
            mesh.RecalculateNormals();
            var meshFilter = GetComponent<MeshFilter>();
            Destroy(meshFilter.sharedMesh);
            meshFilter.sharedMesh = mesh;
        }

        protected override void RebuildAxes()
        {
            if (_axisViews == null || _fromEditor)
                SetupInitialAxisViews();
            AxisTick[] ticks;
            if (_presenter is MultiDimDataPresenter mdp)
                ticks = _presenter.AxisPresenters[0].GenerateFromDimension(mdp.CaptionDimension, _presenter.SelectedMinItem, _presenter.SelectedMaxItem);
            else
                ticks = _presenter.AxisPresenters[0].GenerateFromDiscreteRange(_presenter.SelectedMinItem, _presenter.SelectedMaxItem);
            _axisViews[0].RebuildAxis(ticks);
            ticks = _presenter.AxisPresenters[1].GenerateFromMinMaxValue(0.0f, VisViewHelper.GetGlobalMaximum(_presenter));
            _axisViews[1].RebuildAxis(ticks);
            ticks = _presenter.AxisPresenters[2].GenerateFromDimensionCaptions(_presenter);
            _axisViews[2].RebuildAxis(ticks);
        }
        #endregion
    }
}
