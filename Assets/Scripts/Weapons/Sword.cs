using UnityEngine;

public class Sword : Weapon
{
    public override void Use()
    {
        Debug.Log("The sword is used to attack!");
        //_animator.SetTrigger("Attack");
    }

    public override void Attack(Entity target)
    {
        base.Attack(target);
        Debug.Log("The sword struck the enemy!");
    }
}
