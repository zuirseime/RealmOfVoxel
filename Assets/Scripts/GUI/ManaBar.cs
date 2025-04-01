using UnityEngine;
using UnityEngine.UI;

public class ManaBar : AttributeBar
{
    protected override void Start()
    {
        base.Start();
        _entity.ManaChanged += OnValueChanged;
    }
}