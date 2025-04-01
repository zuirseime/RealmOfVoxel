using UnityEngine;
using UnityEngine.UI;

public class HealthBar : AttributeBar
{
    protected override void Start()
    {
        base.Start();
        _entity.HealthChanged += OnValueChanged;
    }
}
