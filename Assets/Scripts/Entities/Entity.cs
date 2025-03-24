using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    [Header("Stats")]
    [SerializeField] protected EntityAttribute _health;
    [SerializeField] protected EntityAttribute _mana;
    [SerializeField] protected float _attackDamage;

    [Header("Current Stats")]
    [SerializeField, ReadOnly] protected EntityState _currentState;

    protected NavMeshAgent _agent;

    [ReadOnly] public Entity target;

    public NavMeshAgent Agent => _agent;
    public EntityAttribute Health => _health;
    public EntityAttribute Mana => _mana;
    public bool IsAlive => _health.Value > 0;

    public event EventHandler Died;
    public event EventHandler<AttributeEventArgs> HealthChanged;
    public event EventHandler<AttributeEventArgs> ManaChanged;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _health.ValueChanged += (s, e) => HealthChanged?.Invoke(this, e);
        _mana.ValueChanged += (s, e) => ManaChanged?.Invoke(this, e);
    }

    protected virtual void Start()
    {
        _health.Restore();
        _mana.Restore();
    }

    protected virtual void Update()
    {
        _currentState?.Update();
        _mana.Regenerate();
    }

    public virtual void TakeRegularDamage(float amount, float duration, float tick)
    {
        if (!IsAlive || !enabled)
            return;

        StartCoroutine(DamageRoutine(amount, duration, tick));
        if (_health.Value <= 0)
        {
            Die();
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (!IsAlive || !enabled)
            return;

        _health.Drain(amount);
        if (_health.Value <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Died?.Invoke(this, EventArgs.Empty);
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

    private IEnumerator DamageRoutine(float amount, float duration, float tick)
    {
        float timer = 0f;
        while (timer < duration)
        {
            _health.Drain(amount);

            timer += tick * Time.deltaTime;
            yield return null;
        }
    }
}
