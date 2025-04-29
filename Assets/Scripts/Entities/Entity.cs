using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent), typeof(EntityModifiers))]
[RequireComponent(typeof(NavMeshModifier), typeof(Rigidbody))]
public abstract class Entity : MonoBehaviour
{
    [ReadOnly] private Entity _target;

    public event EventHandler<EntityState> StateChanged;
    public event EventHandler<Entity> TargetChanged;

    public Entity Target
    {
        get => _target;
        set
        {
            _target = value;
            if (value != null)
            {
                TargetChanged?.Invoke(this, value);
            }
        }
    }

    [Header("NavMesh Agent")]
    [SerializeField] private float _acceleration = 2f;
    [SerializeField] private float _deceleration = 60f;

    [Header("Attributes")]
    [SerializeField] protected EntityAttribute _health;
    [SerializeField] protected EntityAttribute _mana;

    protected EntityState _currentState;
    protected NavMeshAgent _agent;
    protected EntityState[] _states = new EntityState[0];

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
        }

        TargetChanged += OnTargetChanged;

        Health?.Restore();
        Mana?.Restore();
    }

    protected virtual void Update()
    {
        Agent.acceleration = (Agent.remainingDistance <= 1) ? _deceleration : _acceleration;

        _currentState?.Update();
        Health?.Regenerate();
        Mana?.Regenerate();
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

    public virtual bool CanBeDamagedBy(Entity attacker) => true;

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
        Target = null;
    }

    public void ChangeState<T>() where T : EntityState
    {
        _currentState?.Exit();
        _currentState = _states.FirstOrDefault(s => s is T);
        _currentState?.Enter();
        StateChanged?.Invoke(this, _currentState);
    }

    public bool HasReachedDestionation()
    {
        return !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance && (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f);
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

    protected virtual void OnTargetChanged(object sender, Entity target)
    {
        target.Died += OnTargetDied;
        SetDestination(target.transform.position);
    }
}
