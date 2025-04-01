using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : MonoBehaviour
{
    [SerializeField] private bool _showValue = true;
    [SerializeField] protected Slider _slider;
    [SerializeField] protected Entity _entity;
    [SerializeField] protected TextMeshProUGUI _text;

    private void OnValidate()
    {
        _text.gameObject.SetActive(_showValue);
    }

    protected virtual void Start()
    {
        UpdateText(_entity.Health.Value, _entity.Health.MaxValue);
    }

    public void OnValueChanged(object sender, AttributeEventArgs args)
    {
        UpdateText(args.CurrentValue, args.MaxValue);

        if (_slider == null)
            return;

        _slider.maxValue = args.MaxValue;
        _slider.value = args.CurrentValue;
    }

    private void UpdateText(float value, float maxValue) => _text.text = $"{value:N0} / {maxValue:N0}";
}
