using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis.Input
{
    public abstract class UiElement : MonoBehaviour, IUiElement
    {
        // eat the unity specific mouse events
        //private void OnMouseDown() { }
        //private void OnMouseUp() { }
        public virtual void OnFingerDown(LeanFinger finger)
        {
        }
        public virtual void OnFingerDown(LeanFinger finger, int order, RaycastHit hit)
        {
            OnFingerDown(finger);
        }

        public virtual void OnFingerUp(LeanFinger finger)
        {
        }
        public virtual void OnFingerUp(LeanFinger finger, int order, RaycastHit hit)
        {
            OnFingerUp(finger);
        }

        public virtual void OnFingerMove(LeanFinger finger)
        {
        }

        public virtual void OnFingerMove(LeanFinger finger, int order, RaycastHit hit)
        {
            OnFingerMove(finger);
        }

        public virtual void OnFingerTap(LeanFinger finger)
        {
        }
        public virtual void OnFingerTap(LeanFinger finger, int order, RaycastHit hit)
        {
            OnFingerTap(finger);
        }

        public virtual void OnFingerSwipe(LeanFinger finger)
        {
        }
        public virtual void OnFingerSwipe(LeanFinger finger, int order, RaycastHit hit)
        {
            OnFingerSwipe(finger);
        }

        public virtual void OnFingerPinch(float value)
        {
        }

        public virtual void OnMouseBtnDown(int button)
        {
        }
        public virtual void OnMouseBtnDown(int button, int order, RaycastHit hit)
        {
            OnMouseBtnDown(button);
        }

        public virtual void OnMouseMove(int button)
        {
        }
        public virtual void OnMouseMove(int button, int order, RaycastHit hit)
        {
            OnMouseMove(button);
        }

        public virtual void OnMouseBtnUp(int button)
        {
        }
        public virtual void OnMouseBtnUp(int button, int order, RaycastHit hit)
        {
            OnMouseBtnUp(button);
        }
    }
}
