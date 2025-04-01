using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image _image;
    [SerializeField] protected Vector3 _toolTipOffset;
    [SerializeField] protected TextMeshProUGUI _keybind;
    [SerializeField] protected Tooltip _tooltip;

    protected IDisplayable _displayable;

    public IDisplayable Displayable => _displayable;

    public virtual void Initialize(IDisplayable displayable, string keyBind = "")
    {
        _displayable = displayable;
        
        if (_keybind != null )
        {
            _keybind.text = keyBind;
        }

        if (_displayable != null)
        {
            _image.sprite = _displayable.Sprite;
            _image.color = Color.white;
        } else
        {
            _image.color = Color.clear;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Displayable == null)
            return;

        _tooltip.ShowTooltip(Displayable, transform.position + _toolTipOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Displayable == null)
            return;

        _tooltip.HideTooltip();
    }
}
