using UnityEngine;

public class Bow : Weapon
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private GameObject _arrowSource;

    public override void Use()
    {
        //_animator.SetTrigger("Attack");
    }

    protected override void AttackLogic(Entity target)
    {
        if (_arrowPrefab != null && target != null)
        {
            GameObject arrow = Instantiate(_arrowPrefab, transform.position, transform.rotation);
            arrow.GetComponent<Arrow>().Initialize(target, Damage);
        }
    }
}