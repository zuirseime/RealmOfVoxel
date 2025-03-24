using UnityEngine;
using UnityEngine.UI;

public class HealthBar : AttributeBar
{
    private void Awake()
    {
        _entity.HealthChanged += OnValueChanged;
    }
}
