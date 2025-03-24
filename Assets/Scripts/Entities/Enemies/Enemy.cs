using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Enemy : Entity
{
    [SerializeField] protected float _detectionRange;

    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackCooldown;

    [SerializeField] protected float _wanderRadius;
    [SerializeField] protected float _wanderCooldown;

    public float WanderRadius => _wanderRadius;
    public float WanderCooldown => _wanderCooldown;
    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;

    public void Activate()
    {
        if (!IsAlive) return;

        enabled = true;
        ChangeState(new DummyEnemyWanderState(this));
    }

    protected override void Awake()
    {
        base.Awake();
        GetComponent<SphereCollider>().radius = DetectionRange;
    }

    protected override void Start()
    {
        base.Start();
        enabled = false;
    }

    protected override void Update()
    {
        _currentState?.Update();
    }

    public bool HasPlayerInDetectionRange()
    {
        return target != null && target.IsAlive && DistanceToTarget(target) < _detectionRange;
    }

    public bool HasPlayerInAttackRange()
    {
        return target != null && target.IsAlive && DistanceToTarget(target) < _attackRange;
    }

    private float DistanceToTarget(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    protected override void Die()
    {
        base.Die();
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
