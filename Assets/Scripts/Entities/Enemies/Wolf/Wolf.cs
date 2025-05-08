using UnityEngine;
using UnityEngine.AI;

public class Wolf : Enemy
{
    [Header("Wolf Specific")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _stunDuration;
    [SerializeField] private float _retreatDuration;

    public float StunDuration => _stunDuration;
    public float RetreatDuration => _retreatDuration;

    public override void Activate()
    {
        base.Activate();
        ChangeState<WolfWanderState>();
    }

    public void JumpTowardsTarget()
    {
        if (Target == null)
            return;
        Vector3 direction = (Target.transform.position - transform.position).normalized;
        Agent.velocity = direction * _jumpForce;
    }

    public void RetreatFromTarget()
    {
        if (Target == null)
            return;
        Vector3 direction = (transform.position - Target.transform.position).normalized;
        Agent.velocity = direction * _jumpForce;
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[] {
            new WolfWanderState(this),
            new WolfFlankState(this),
            new WolfAttackState(this),
            new WolfRetreatState(this)
        };
    }
}
