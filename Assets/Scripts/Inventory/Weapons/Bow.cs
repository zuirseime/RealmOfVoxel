using UnityEngine;

public class Bow : Weapon
{
    [SerializeField] private Projectile _arrowPrefab;

    protected override void AttackLogic(Entity target)
    {
        if (_arrowPrefab != null && target != null)
        {
            Projectile arrow = Instantiate(_arrowPrefab, transform.position, transform.rotation);
            arrow.Initialize(target, Damage, Owner);
        }
    }
}