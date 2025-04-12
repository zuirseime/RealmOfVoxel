using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Enemy : Entity
{
    [SerializeField] protected float _detectionRange;

    [SerializeField] protected float _attackDamage;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackCooldown;

    [SerializeField] protected float _wanderRadius;
    [SerializeField] protected float _wanderCooldown;

    [SerializeField] protected int _minCoins;
    [SerializeField] protected int _maxCoins;

    public float WanderRadius => _wanderRadius;
    public float WanderCooldown => _wanderCooldown;
    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    public int Coins { get; private set; }

    public virtual void Activate()
    {
        if (!IsAlive) return;

        enabled = true;
    }

    public override void Attack()
    {
        base.Attack();
        if (target != null)
        {
            target.TakeDamage(this, _attackDamage * GetComponent<EntityModifiers>().DamageModifier.BaseValue);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        GetComponent<SphereCollider>().radius = DetectionRange;

        Coins = Random.Range(_minCoins, _maxCoins);

        enabled = false;
    }

    protected override void Update()
    {
        _currentState?.Update();
    }

    public bool HasPlayerInDetectionRange()
    {
        return target != null && target.IsAlive;
    }

    public bool HasPlayerInAttackRange()
    {
        return HasPlayerInDetectionRange() && DistanceToTarget(target) < _attackRange;
    }

    private float DistanceToTarget(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    protected override void Die(Entity entity)
    {
        if (entity.TryGetComponent(out Inventory inventory))
        {
            inventory.AddCoins(Coins);
        }

        base.Die(entity);
        transform.Rotate(Vector3.right * 90, Space.Self);
        Agent.enabled = false;
        this.enabled = false;
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) && player.IsAlive)
        {
            target = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player) && player.IsAlive)
        {
            target = null;
        }
    }
}
