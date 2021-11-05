using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorToggler : MonoBehaviour
{
    [SerializeField] private Color _colorToChange;

    private Image _image;
    private Color _originalColor;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _originalColor = _image.color;
    }

    public void ToggleColor()
    {
        var currentColor = _image.color;
        _image.color = currentColor == _originalColor ? _colorToChange : _originalColor;
    }
}