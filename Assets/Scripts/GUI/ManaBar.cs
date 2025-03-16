using UnityEngine;
using UnityEngine.UI;

public class ManaBar : AttributeBar
{
    private void Start()
    {
        _entity.ManaChanged += OnValueChanged;
    }
}