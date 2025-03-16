using UnityEngine;

public class Bow : Weapon
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private GameObject _arrowSource;

    public override void Use()
    {
        Debug.Log("The bow is used to attack!");
        //_animator.SetTrigger("Attack");
    }

    public override void Attack(Entity target)
    {
        if (!CanAttack(target))
            return;

        if (Time.time < _nextAttackTime)
            return;

        if (_arrowPrefab != null && target != null)
        {
            GameObject arrow = Instantiate(_arrowPrefab, transform.position, transform.rotation);
            arrow.GetComponent<Arrow>().Initialize(target, damage);
            _nextAttackTime = Time.time + cooldown;
        }
    }
}
