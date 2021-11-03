using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextToggler : MonoBehaviour
{
    [SerializeField] private string _textToChange;
    private string _orignalText;
    private TextMeshProUGUI _textBox;

    private void Awake()
    {
        _textBox = GetComponent<TextMeshProUGUI>();
        _orignalText = _textBox.text;
    }

    public void ToggleText()
    {
        var currentText = _textBox.text;
        _textBox.text = currentText == _textToChange ? _orignalText : _textToChange;
    }
}
