using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(EntityModifiers))]
public abstract class Entity : MonoBehaviour
{
    [ReadOnly] public Entity target;

    [Header("NavMesh Agent")]
    [SerializeField] private float _acceleration = 2f;
    [SerializeField] private float _deceleration = 60f;

    [Header("Attributes")]
    [SerializeField] protected EntityAttribute _health;
    [SerializeField] protected EntityAttribute _mana;

    protected EntityState _currentState;
    protected NavMeshAgent _agent;

    private bool _takingPeriodicDamage;

    public NavMeshAgent Agent => _agent;
    public EntityAttribute Health => _health;
    public EntityAttribute Mana => _mana;
    public bool IsAlive { get; private set; } = true;

    public event EventHandler Died;
    public event EventHandler<AttributeEventArgs> HealthChanged;
    public event EventHandler<AttributeEventArgs> ManaChanged;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (Health != null && Mana != null)
        {
            Health.ValueChanged += (s, e) => HealthChanged?.Invoke(this, e);
            Mana.ValueChanged += (s, e) => ManaChanged?.Invoke(this, e);

            Health.Restore();
            Mana.Restore();
        }
    }

    protected virtual void Update()
    {
        Agent.acceleration = (Agent.remainingDistance <= 1) ? _deceleration : _acceleration;

        _currentState?.Update();
        Health.Regenerate();
        Mana.Regenerate();
    }

    public virtual void TakePeriodicDamage(Entity entity, float amount, float duration, float tick)
    {
        if (_takingPeriodicDamage)
        {
            TakeDamage(entity, amount);
            return;
        }

        StartCoroutine(DamageRoutine(entity, amount, duration, tick));
    }

    public virtual void TakeDamage(Entity entity, float amount)
    {
        if (!IsAlive || !enabled)
            return;

        float damageAfterDefence = amount / GetComponent<EntityModifiers>().DefenceModifier.Value;
        Health.Drain(damageAfterDefence);
        if (Health.Value <= 0)
        {
            Die(entity);
        }
    }

    protected virtual void Die(Entity entity)
    {
        IsAlive = Health.Value > 0;
        Died?.Invoke(this, EventArgs.Empty);
    }

    public virtual void Attack() { }

    public virtual void AlternativeAttack() { }

    public void Heal(float amount)
    {
        _health.Regenerate(amount: amount);
    }

    public void RestoreMana(float amount)
    {
        _mana.Regenerate(amount);
    }

    public void DrainMana(float amount)
    {
        _mana.Drain(amount);
    }

    public void SetDestination(Vector3 destination)
    {
        Agent.SetDestination(destination);
    }

    public void ClearDestination()
    {
        Agent.isStopped = true;
        Agent.ResetPath();
    }

    public void OnTargetDied(object sender, EventArgs args)
    {
        target = null;
    }

    public void ChangeState(EntityState state)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    private IEnumerator DamageRoutine(Entity entity, float amount, float duration, float tick)
    {
        _takingPeriodicDamage = true;
        float timer = 0f;
        while (timer < duration && IsAlive)
        {
            TakeDamage(entity, amount);

            timer += tick;
            yield return new WaitForSeconds(tick);
        }
        _takingPeriodicDamage = false;
    }
}
