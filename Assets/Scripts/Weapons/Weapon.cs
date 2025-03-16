using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage;
    public float range;
    [SerializeField] protected float _speed;
    public float cooldown;

    protected float _nextAttackTime;
    protected Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void Attack(Entity target)
    {
        if (!CanAttack(target))
            return;

        if (Time.time < _nextAttackTime)
            return;

        target.TakeDamage(damage);
        _nextAttackTime = Time.time + cooldown;
    }

    public virtual bool CanAttack(Entity target)
    {
        return target != null && Vector3.Distance(transform.position, target.transform.position) <= range;
    }

    public abstract void Use();
}
