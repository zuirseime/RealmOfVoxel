using UnityEngine;

public class Torch : Weapon
{
    [SerializeField] private float _duration;
    [SerializeField] private float _tickRate;

    protected override void AttackLogic(Entity target)
    {
        target.TakePeriodicDamage(Owner, Damage, _duration, _tickRate);
    }
}
