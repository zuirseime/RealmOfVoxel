using System;
using UnityEngine;

[Serializable]
public class EntityAttribute
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _regenerationRate;

    [field: SerializeField, ReadOnly] public float Value { get; set; } = 0;
    public float MaxValue => _maxValue;

    public event EventHandler<AttributeEventArgs> ValueChanged;

    public void Restore() => SetValue(MaxValue);
    public void Drain(float amount) => SetValue(Mathf.Max(Value - amount, 0));
    public void Regenerate(float multiplier = 1) => Regenerate(_regenerationRate * Time.deltaTime, multiplier);
    public void Regenerate(float amount, float multiplier = 1) => SetValue(Mathf.Min(Value + amount * multiplier, MaxValue));

    public bool CanDrain(float amount) => Value >= amount;
    public bool CanRegenerate(float amount) => Value + amount <= MaxValue;

    private void SetValue(float newValue)
    {
        float previous = Value;
        if (Math.Abs(Value - newValue) > Mathf.Epsilon)
        {
            Value = newValue;
            ValueChanged?.Invoke(this, new AttributeEventArgs(Value, previous, MaxValue));
        }
    }
}