using Lean.Touch;
using UnityEngine;

namespace u2vis.Input
{
    public interface IUiElement
    {
        void OnFingerDown(LeanFinger finger, int order, RaycastHit hit);
        void OnFingerMove(LeanFinger finger, int order, RaycastHit hit);
        void OnFingerUp(LeanFinger finger, int order, RaycastHit hit);
        void OnFingerTap(LeanFinger finger, int order, RaycastHit hit);
        void OnFingerSwipe(LeanFinger finger, int order, RaycastHit hit);
        void OnFingerPinch(float value);
        void OnMouseBtnDown(int button, int order, RaycastHit hit);
        void OnMouseMove(int button, int order, RaycastHit hit);
        void OnMouseBtnUp(int button, int order, RaycastHit hit);
    }
}
