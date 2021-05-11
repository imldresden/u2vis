using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace u2vis
{
    public class BarChart3D_Interaction : BaseVisualizationInteraction
    {
        public override void OnMouseBtnUp(int button, int order, RaycastHit hit)
        {
            base.OnMouseBtnUp(button, order, hit);

            var barChart = _visualization as BarChart3D;
            if (barChart == null)
                return;

            int offset = (int)_presenter.SelectedMinItem;
            int length = (int)_presenter.SelectedMaxItem - offset;
            int trisPerBar = barChart.DataItemMesh.triangles.Length / 3;
            int index = hit.triangleIndex / trisPerBar;

            var dimIndex = index / length;
            var itemIndex = index - dimIndex * length;
            _presenter.ToogleItemHighlight(itemIndex);
        }
    }
}
