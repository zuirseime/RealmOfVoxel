using UnityEngine;

public class Sword : Weapon
{
    protected override void AttackLogic(Entity target)
    {
        target.TakeDamage(Owner, Damage);
    }
}
