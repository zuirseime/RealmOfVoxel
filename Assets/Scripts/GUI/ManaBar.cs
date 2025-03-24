using UnityEngine;
using UnityEngine.UI;

public class ManaBar : AttributeBar
{
    private void Awake()
    {
        _entity.ManaChanged += OnValueChanged;
    }
}