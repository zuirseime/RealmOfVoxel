using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] protected Entity _entity;

    public void OnValueChanged(object sender, AttributeEventArgs args)
    {
        if (_slider == null)
            return;

        if (_slider.maxValue != args.MaxValue)
        {
            _slider.maxValue = args.MaxValue;
        }

        _slider.value = args.CurrentValue;
    }
}
