using Lean.Touch;
using UnityEngine;

namespace u2vis
{
    public class BarChart2D_Interaction : BaseVisualizationInteraction
    {
        protected virtual void ToggleHighlight(RaycastHit hit)
        {
            int offset = _presenter.SelectedMinItem;
            int length = _presenter.SelectedMaxItem - offset;
            var pos = transform.InverseTransformPoint(hit.point);
            int index = (int)(pos.x / _visualization.Size.x * length) + offset;
            float value = VisViewHelper.GetItemValueAbsolute(_presenter, 1, index, true);
            if (pos.y < value * _visualization.Size.y)
                _presenter.ToogleItemHighlight(index);
            //Debug.Log("itemIndex=" + index + ", value=" + value + ", pos.y=" + pos.y + ", nPos.y=" + (pos.y / _visualization.Size.y));
        }

        public override void OnMouseBtnUp(int button, int order, RaycastHit hit)
        {
            base.OnMouseBtnUp(button, order, hit);
            ToggleHighlight(hit);
        }

        public override void OnFingerTap(LeanFinger finger, int order, RaycastHit hit)
        {
            base.OnFingerTap(finger, order, hit);
            ToggleHighlight(hit);
        }
    }
}
