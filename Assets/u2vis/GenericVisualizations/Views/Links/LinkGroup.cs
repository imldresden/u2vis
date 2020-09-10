using UnityEngine;

namespace u2vis
{
    public class LinkGroup : MonoBehaviour
    {
        [SerializeField]
        protected GenericDataPresenter[] _linkedPresenters;

        protected virtual void OnEnable()
        {
            foreach (var p in _linkedPresenters)
            {
                p.HighlightChanged += Presenter_HighlightChanged;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var p in _linkedPresenters)
            {
                p.HighlightChanged -= Presenter_HighlightChanged;
            }
        }

        protected virtual void Presenter_HighlightChanged(GenericDataPresenter sender, int itemIndex)
        {
            foreach (var p in _linkedPresenters)
                if (p != sender)
                {
                    p.HighlightChanged -= Presenter_HighlightChanged;
                    p.ToogleItemHighlight(itemIndex);
                    p.HighlightChanged += Presenter_HighlightChanged;
                }
        }

        public virtual void Clear()
        {
            foreach (var p in _linkedPresenters)
                p.ClearHighlights();
        }
    }
}
