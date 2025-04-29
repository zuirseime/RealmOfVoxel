using UnityEngine;
using UnityEngine.UI;

public class ManaBar : AttributeBar
{
    protected void Start()
    {
        UpdateText(_entity.Mana.Value, _entity.Mana.MaxValue);

        if (_entity != null)
            _entity.ManaChanged += OnValueChanged;
    }
}