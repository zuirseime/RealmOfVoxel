using UnityEngine;
using UnityEngine.UI;

public class HealthBar : AttributeBar
{
    protected void Start()
    {
        UpdateText(_entity.Health.Value, _entity.Health.MaxValue);
        if (_entity != null)
            _entity.HealthChanged += OnValueChanged;
    }
}
