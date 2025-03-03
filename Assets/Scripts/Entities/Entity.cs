using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    [Header("Stats")]
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _maxMana;
    [SerializeField] protected float _attackDamage;

    protected float _currentHealth;
    protected float _currentMana;

    public NavMeshAgent agent;

    public bool IsAlive => _currentHealth > 0;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        _currentHealth = _maxHealth;
        _currentMana = _maxMana;
    }

    public virtual void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die() { }

    public virtual void Attack(Entity target)
    {
        if (target != null)
        {
            target.TakeDamage(_attackDamage);
        }
    }

    public virtual void AlternativeAttack(Entity target) { }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
    }

    public void RestoreMana(float amount)
    {
        _currentHealth = Mathf.Min(_currentMana + amount, _maxMana);
    }
}
