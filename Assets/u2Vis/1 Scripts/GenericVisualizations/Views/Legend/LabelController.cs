using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelController : MonoBehaviour
{
    [SerializeField]
    private Image _colorQuad;
    [SerializeField]
    private Text _text;
    [SerializeField]
    private RectTransform _labelCanvas;

    public void SetLabelText(string newText)
    {
        _text.text = newText;
    }

    public void SetLabelColor(Color newColor)
    {
        _colorQuad.color = newColor;
    }

    public float GetHeight()
    {
        return _labelCanvas.rect.height * _labelCanvas.localScale.y;
    }

    public void SetWidth(float width)
    {
        _labelCanvas.sizeDelta = new Vector2(width/0.001f, _labelCanvas.sizeDelta.y);
    }
}
