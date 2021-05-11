using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace u2vis.Input
{
    public class ToggleButton : UiElement
    {
        [SerializeField]
        private bool _toggled = false;
        [SerializeField]
        private UnityEvent OnToggleOn = null;
        [SerializeField]
        private UnityEvent OnToggleOff = null;
        [SerializeField]
        private Color _defaultColor = Color.white;
        [SerializeField]
        private Color _toggledColor = Color.red;

        private void Start()
        {
            var image = GetComponent<Image>();
            if (image != null)
                image.color = _toggled ? _toggledColor : _defaultColor;
        }

        public override void OnMouseBtnUp(int button)
        {
            SetToggle(!_toggled);
        }
        public override void OnFingerTap(LeanFinger finger)
        {
            SetToggle(!_toggled);
        }

        public void SetToggle(bool state)
        {
            if (state == _toggled)
                return;
            _toggled = state;

            if (_toggled)
            {
                if (OnToggleOn != null)
                    OnToggleOn.Invoke();
                var image = GetComponent<Image>();
                if (image != null)
                    image.color = _toggledColor;
            }
            else
            {
                if (OnToggleOff != null)
                    OnToggleOff.Invoke();
                var image = GetComponent<Image>();
                if (image != null)
                    image.color = _defaultColor;
            }
        }
    }
}
