using UnityEngine;

public class Sword : Weapon
{
    public override void Use()
    {
        //_animator.SetTrigger("Attack");
    }

    protected override void AttackLogic(Entity target)
    {
        target.TakeDamage(Damage);
    }
}
