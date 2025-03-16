using System;
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
    public Entity target;

    [field: Header("Current Stats")]
    [field: SerializeField] public float Health
    {
        get => _currentHealth;
        private set
        {
            if (_currentHealth != value)
            {
                _currentHealth = value;
                HealthChanged?.Invoke(this, new AttributeEventArgs(value, _maxHealth));
            }
        }
    }

    [field: SerializeField] public float Mana
    {
        get => _currentMana;
        private set
        {
            if (_currentMana != value)
            {
                _currentMana = value;
                ManaChanged?.Invoke(this, new AttributeEventArgs(value, _maxMana));
            }
        }
    }

    public bool IsAlive => Health > 0;

    public event EventHandler EntityDied;
    public event EventHandler<AttributeEventArgs> HealthChanged;
    public event EventHandler<AttributeEventArgs> ManaChanged;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        Health = _maxHealth;
        Mana = _maxMana;
    }

    public virtual void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        EntityDied?.Invoke(this, EventArgs.Empty);
    }

    public virtual void Attack()
    {
        if (target != null)
        {
            target.TakeDamage(_attackDamage);
        }
    }

    public virtual void AlternativeAttack() { }

    public void Heal(float amount)
    {
        Health = Mathf.Min(Health + amount, _maxHealth);
    }

    public void RestoreMana(float amount)
    {
        Mana = Mathf.Min(Mana + amount, _maxMana);
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void ClearDestination()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }
}

public class AttributeEventArgs : EventArgs
{
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }

    public AttributeEventArgs(float value, float max)
    {
        CurrentValue = value;
        MaxValue = max;
    }
}