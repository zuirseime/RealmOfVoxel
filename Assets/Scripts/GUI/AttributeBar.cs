using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : MonoBehaviour
{
    [SerializeField] private bool _showValue = true;
    [SerializeField] private Slider _slider;
    [SerializeField] protected Entity _entity;
    [SerializeField] protected TextMeshProUGUI _text;

    private void OnValidate()
    {
        _text.gameObject.SetActive(_showValue);
    }

    public void OnValueChanged(object sender, AttributeEventArgs args)
    {
        _text.text = $"{args.CurrentValue:N0} / {args.MaxValue:N0}";

        if (_slider == null)
            return;

        if (_slider.maxValue != args.MaxValue)
        {
            _slider.maxValue = args.MaxValue;
        }

        _slider.value = args.CurrentValue;
    }
}
