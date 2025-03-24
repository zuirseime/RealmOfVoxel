using UnityEngine;

public class Torch : Weapon
{
    [SerializeField] private float _duration;
    [SerializeField] private float _tickRate;

    public override void Use() { }

    protected override void AttackLogic(Entity target)
    {
        if (!target.TakingPeriodicDamage)
            target.TakePeriodicDamage(Damage, _duration, _tickRate);
        else
            target.TakeDamage(Damage);
    }
}
