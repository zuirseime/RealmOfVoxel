using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Color Preferences")]
    [SerializeField] private Color _defaultTextColor;
    [SerializeField] private Color _highlightTextColor;
    [SerializeField] private Color _disabledTextColor;

    [Header("Size Preferences")]
    [SerializeField, ReadOnly] private float _defaultSize;
    [SerializeField] private float _highlightSize;

    private TextMeshProUGUI _text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_text != null)
        {
            _text.color = _highlightTextColor;
            _text.fontSize = _highlightSize;
            _text.fontStyle = FontStyles.Bold;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_text != null)
        {
            _text.color = _defaultTextColor;
            _text.fontSize = _defaultSize;
            _text.fontStyle = FontStyles.Normal;
        }
    }

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _defaultSize = _text.fontSize;
        _text.color = _defaultTextColor;
        GetComponent<Button>().enabled = true;
    }

    private void OnDisable()
    {
        _text.color = _disabledTextColor;
        GetComponent<Button>().enabled = false;
    }
}
