using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis.Input
{
    public sealed class u2visInputModule : MonoBehaviour
    {
        private Dictionary<IUiElement, List<LeanFinger>> _touchesByElement = new Dictionary<IUiElement, List<LeanFinger>>();

        [SerializeField]
        private bool _simulateMouseWithTouches = false;

        private void OnEnable()
        {
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerUp += OnFingerUp;
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerSwipe += OnFingerSwipe;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerUp -= OnFingerUp;
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerSwipe -= OnFingerSwipe;
        }

        private void Start()
        {
            UnityEngine.Input.simulateMouseWithTouches = _simulateMouseWithTouches;
        }

        private void Update()
        {
            for (int i = 0; i < 3; i++)
            {
                if (UnityEngine.Input.GetMouseButtonDown(i))
                    OnMouseDown(i);
                if (UnityEngine.Input.GetMouseButton(i))
                    OnMouseMove(i);
                if (UnityEngine.Input.GetMouseButtonUp(i))
                    OnMouseUp(i);
            }

            foreach (var kvp in _touchesByElement)
            {
                foreach (var finger in kvp.Value)
                    OnFingerMove(kvp.Key, finger);
                if (kvp.Value.Count > 1)
                    OnFingerPinch(kvp.Key, kvp.Value);
            }
        }

        private void AddFinger(IUiElement uie, LeanFinger finger)
        {
            List<LeanFinger> fingers = null;
            if (!_touchesByElement.TryGetValue(uie, out fingers))
            {
                fingers = new List<LeanFinger>();
                _touchesByElement.Add(uie, fingers);
            }
            fingers.Add(finger);
        }

        private void RemoveFinger(IUiElement uie, LeanFinger finger)
        {
            List<LeanFinger> fingers = null;
            if (_touchesByElement.TryGetValue(uie, out fingers))
            {
                fingers.Remove(finger);
                if (fingers.Count == 0)
                    _touchesByElement.Remove(uie);
            }
        }

        private void OnFingerDown(LeanFinger finger)
        {
            if (GetUiElements(finger.ScreenPosition, out List<UiElemHitResult> results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    AddFinger(res.UiElement, finger);
                    res.UiElement.OnFingerDown(finger, i, res.HitResult);
                }
        }

        private void OnFingerMove(IUiElement uie, LeanFinger finger)
        {
            if (GetUiElements(finger.ScreenPosition, out List<UiElemHitResult> results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    res.UiElement.OnFingerMove(finger, i, res.HitResult);
                }
        }

        private void OnFingerUp(LeanFinger finger)
        {
            List<UiElemHitResult> results;
            if (GetUiElements(finger.ScreenPosition, out results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    RemoveFinger(res.UiElement, finger);
                    res.UiElement.OnFingerUp(finger, i, res.HitResult);
                }
            // rerun the query on the original starting position so those elements get the
            // finger up event even if the finger is no longer there
            if (GetUiElements(finger.StartScreenPosition, out List<UiElemHitResult> orgHits))
            {
                for (int i = 0; i < orgHits.Count; i++)
                {
                    var orgRes = orgHits[i];
                    bool duplicate = false;
                    foreach (var res in results)
                        if (res.UiElement == orgRes.UiElement)
                        {
                            duplicate = true;
                            break;
                        }
                    if (duplicate)
                        continue;
                    RemoveFinger(orgRes.UiElement, finger);
                    orgRes.UiElement.OnFingerUp(finger, 0, orgRes.HitResult);
                }
            }
        }

        private void OnFingerTap(LeanFinger finger)
        {
            if (GetUiElements(finger.ScreenPosition, out List<UiElemHitResult> results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    res.UiElement.OnFingerTap(finger, i, res.HitResult);
                }
        }

        private void OnFingerSwipe(LeanFinger finger)
        {
            if (GetUiElements(finger.ScreenPosition, out List<UiElemHitResult> results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    res.UiElement.OnFingerSwipe(finger, i, res.HitResult);
                }
        }

        private void OnFingerPinch(IUiElement uie, List<LeanFinger> fingers)
        {
            float pinchScale = LeanGesture.GetPinchScale(fingers);
            uie.OnFingerPinch(pinchScale);
        }

        private void OnMouseDown(int button)
        {
            if (GetUiElements(UnityEngine.Input.mousePosition, out List<UiElemHitResult> results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    res.UiElement.OnMouseBtnDown(button, i, res.HitResult);
                }
        }

        private void OnMouseMove(int button)
        {
            if (GetUiElements(UnityEngine.Input.mousePosition, out List<UiElemHitResult> results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    res.UiElement.OnMouseMove(button, i, res.HitResult);
                }
        }

        private void OnMouseUp(int button)
        {
            if (GetUiElements(UnityEngine.Input.mousePosition, out List<UiElemHitResult> results))
                for (int i = 0; i < results.Count; i++)
                {
                    var res = results[i];
                    res.UiElement.OnMouseBtnUp(button, i, res.HitResult);
                }
        }

        private bool GetUiElement(Vector3 screenPosition, out IUiElement uiElement, out RaycastHit hit)
        {
            uiElement = null;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray.origin, ray.direction, out hit))
                return false;
            uiElement = hit.transform.GetComponent<IUiElement>();
            if (uiElement == null)
                return false;
            return true;
        }

        private bool GetUiElements(Vector3 screenPosition, out List<UiElemHitResult> uiElements)
        {
            uiElements = new List<UiElemHitResult>();
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            var hits = Physics.RaycastAll(ray);
            foreach (var hit in hits)
            {
                var uiElem = hit.transform.GetComponent<IUiElement>();
                if (uiElem != null)
                    uiElements.Add(new UiElemHitResult(uiElem, hit));
            }
            if (uiElements.Count == 0)
                return false;
            uiElements.Sort(UiElemHitResult.Compare);
            return true;
        }

        private class UiElemHitResult
        {
            public readonly IUiElement UiElement;
            public readonly RaycastHit HitResult;

            public UiElemHitResult(IUiElement uiElement, RaycastHit hitResult)
            {
                UiElement = uiElement;
                HitResult = hitResult;
            }

            public static int Compare(UiElemHitResult x, UiElemHitResult y)
            {
                return Comparer<float>.Default.Compare(x.HitResult.distance, y.HitResult.distance);
            }
        }
    }
}