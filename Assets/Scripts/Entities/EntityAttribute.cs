using System;
using UnityEngine;

[Serializable]
public class EntityAttribute
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _regenerationRate;
    [SerializeField, ReadOnly] private float _value = 0;

    public float Value
    {
        get => _value;
        set
        {
            float previous = _value;
            if (Math.Abs(_value - value) > Mathf.Epsilon)
            {
                _value = value;
                ValueChanged?.Invoke(this, new AttributeEventArgs(Value, previous, MaxValue));
            }
        }
    }
    public float MaxValue
    {
        get => _maxValue;
        set => _maxValue = value;
    }

    public event EventHandler<AttributeEventArgs> ValueChanged;

    public void Restore() => Value = MaxValue;
    public void Drain(float amount) => Value = Mathf.Max(Value - amount, 0);
    public void Regenerate(float multiplier = 1) => Regenerate(_regenerationRate * Time.deltaTime, multiplier);
    public void Regenerate(float amount, float multiplier = 1) => Value = Mathf.Min(Value + amount * multiplier, MaxValue);

    public bool CanDrain(float amount) => Value >= amount;
}