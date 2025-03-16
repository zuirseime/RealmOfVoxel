using UnityEngine;
using UnityEngine.UI;

public class HealthBar : AttributeBar
{
    private void Start()
    {
        _entity.HealthChanged += OnValueChanged;
    }
}
