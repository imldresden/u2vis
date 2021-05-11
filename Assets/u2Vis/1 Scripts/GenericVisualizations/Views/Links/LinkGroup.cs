using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Class that consists of a list of several data presenters that are linked´together to share, e.g., highlights.
    /// </summary>
    public class LinkGroup : MonoBehaviour
    {
        [SerializeField]
        protected GenericDataPresenter[] _linkedPresenters;

        #region Protected Methods
        /// <summary>
        /// Called by Unity when the script is enabled.
        /// Sets all event handlers to listen to the contained presenters.
        /// </summary>
        protected virtual void OnEnable()
        {
            foreach (var p in _linkedPresenters)
            {
                p.HighlightChanged += Presenter_HighlightChanged;
            }
        }
        /// <summary>
        /// Called by Unity when the script is disabled.
        /// Removes all event handlers that listen to the contained presenters.
        /// </summary>
        protected virtual void OnDisable()
        {
            foreach (var p in _linkedPresenters)
            {
                p.HighlightChanged -= Presenter_HighlightChanged;
            }
        }
        /// <summary>
        /// Called whenver the highlight of a presenter contained in this link group has changed.
        /// Notifies all other presenters of the change so they can sync their state.
        /// </summary>
        /// <param name="sender">The presenter which has changed.</param>
        /// <param name="itemIndex">The index of the value which hightlight has changed.</param>
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
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the specifief data presenter to this LinkGroup.
        /// </summary>
        /// <param name="presenter">The data presenters that should be added.</param>
        public virtual void Add(GenericDataPresenter presenter)
        {
            if (presenter == null)
                throw new ArgumentException("presenter can't be null");
            if (_linkedPresenters == null && _linkedPresenters.Length == 0)
            {
                _linkedPresenters = new GenericDataPresenter[] { presenter };
                return;
            }
            var newList = new GenericDataPresenter[_linkedPresenters.Length + 1];
            Array.Copy(_linkedPresenters, 0, newList, 0, _linkedPresenters.Length);
            newList[_linkedPresenters.Length] = presenter;
            _linkedPresenters = newList;
            presenter.HighlightChanged += Presenter_HighlightChanged;
        }
        /// <summary>
        /// Removes the specified data presenter from this lin group.
        /// </summary>
        /// <param name="presenter">The presenter that should be removed.</param>
        /// <returns>true if the presenter was removed otherwise false.</returns>
        public virtual bool Remove(GenericDataPresenter presenter)
        {
            var tmp = new List<GenericDataPresenter>(_linkedPresenters);
            if (!tmp.Remove(presenter))
                return false;
            _linkedPresenters = tmp.ToArray();
            presenter.HighlightChanged -= Presenter_HighlightChanged;
            return true;
        }
        /// <summary>
        /// Clears the list of presenters
        /// </summary>
        public virtual void ClearPresenters()
        {
            _linkedPresenters = new GenericDataPresenter[0];
        }
        /// <summary>
        /// Clears the highlights of all linked presenters.
        /// </summary>
        public virtual void ClearHighlights()
        {
            foreach (var p in _linkedPresenters)
                p.ClearHighlights();
        }
        #endregion
    }
}
