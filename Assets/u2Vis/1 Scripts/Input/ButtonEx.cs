using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

namespace u2vis.Input
{
    public class ButtonEx : UiElement
    {
        [SerializeField]
        private UnityEvent Clicked = null;
        [SerializeField]
        private UnityEvent FingerDown = null;
        [SerializeField]
        private UnityEvent FingerUp = null;
        [SerializeField]
        private UnityEvent FingerTap = null;

        public override void OnMouseBtnUp(int button)
        {
            if (Clicked != null)
                Clicked.Invoke();
        }

        public override void OnFingerDown(LeanFinger finger)
        {
            if (FingerDown != null)
                FingerDown.Invoke();
        }

        public override void OnFingerUp(LeanFinger finger)
        {
            if (FingerUp != null)
                FingerUp.Invoke();
            if (Clicked != null)
                Clicked.Invoke();
        }

        public override void OnFingerTap(LeanFinger finger)
        {
            if (FingerTap != null)
                FingerTap.Invoke();
        }
    }
}
