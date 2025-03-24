using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : AuraEffect
{
    public float RotationSpeed { get; set; }
    public float Absorbtion { get; set; }

    protected override void Start()
    {
        base.Start();
        Owner.HealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(object sender, AttributeEventArgs args)
    {
        float absorbedDamage = args.PreviousValue - args.CurrentValue;
        if (absorbedDamage < 0) return;

        float reducedDamage = absorbedDamage * (1 - Absorbtion / 100);
        Owner.Heal(reducedDamage);
    }

    protected override void Update()
    {
        base.Update();
        transform.position = Owner.transform.position;
        transform.Rotate(RotationSpeed * Time.deltaTime * Vector3.up);
    }

    private void OnDestroy()
    {
        Owner.HealthChanged -= OnHealthChanged;
    }
}
