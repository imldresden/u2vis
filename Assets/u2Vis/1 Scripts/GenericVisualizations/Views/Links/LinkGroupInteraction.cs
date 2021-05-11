using u2vis.Input;
using Lean.Touch;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Interaction component for link groups which can be used a an example.
    /// Disables all current highlights whenever a double tap is performed.
    /// </summary>
    public class LinkGroupInteraction : UiElement
    {
        #region
        /// <summary>
        /// The link group which this interaction script handles.
        /// </summary>
        [SerializeField]
        protected LinkGroup _linkGroup = null;
        /// <summary>
        /// The time intervall used to determine when a touble tap has been performed.
        /// </summary>
        [SerializeField]
        protected float _doubleTapIntervall = 0.5f;
        /// <summary>
        /// Timer variable which is set after a tap was performed to determine if the next tap stays within the double tap intervall.
        /// </summary>
        protected float _doubleTapTimer = -1;
        #endregion

        #region Public Properties
        public LinkGroup Lingroup
        {
            get { return _linkGroup; }
            set { _linkGroup = value; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Called by Unity every frame.
        /// </summary>
        protected void Update()
        {
            if (_doubleTapTimer > 0)
            {
                _doubleTapTimer -= Time.deltaTime;
                if (_doubleTapTimer <= 0)
                    _doubleTapTimer = -1;
            }
        }
        /// <summary>
        /// Called whenever a touch collided with the collider this script is attached to.
        /// </summary>
        /// <param name="finger">The finger object representing the touch.</param>
        /// <param name="order">The order in which this collider was hit.</param>
        /// <param name="hit">The raycast hit which was used to determine the hit.</param>
        public override void OnFingerDown(LeanFinger finger, int order, RaycastHit hit)
        {
            base.OnFingerUp(finger, order, hit);
            if (order > 1)
                return;

            if (_doubleTapTimer <= -1)
            {
                _doubleTapTimer = _doubleTapIntervall;
            }
            else
            {
                ClearLinks();
                _doubleTapTimer = -1;
            }
        }
        /// <summary>
        /// Clear the links of thi
        /// </summary>
        protected virtual void ClearLinks()
        {
            if (_linkGroup != null)
                _linkGroup.ClearHighlights();
        }
        #endregion
    }
}
