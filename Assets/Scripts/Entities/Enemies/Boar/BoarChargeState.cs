using UnityEngine;

public class BoarChargeState : EnemyState<Boar>
{
    private Vector3 _chargeTarget;
    private bool _charged;

    public BoarChargeState(Boar boar) : base(boar) { }

    public override void Enter()
    {
        _charged = false;

        Vector3 direction = (_enemy.Target.transform.position - _enemy.transform.position).normalized;
        float distance = Vector3.Distance(_enemy.transform.position, _enemy.Target.transform.position) + _enemy.ChargeDistance / 2f;
        NavMeshUtils.CheckAndAdjustPointOnNavMesh(_enemy.transform.position, direction, distance, out _chargeTarget);

        _enemy.Agent.speed = _enemy.ChargeSpeed;
        _enemy.SetDestination(_chargeTarget);
    }

    public override void Update()
    {
        if (_enemy.HasPlayerInAttackRange() && !_charged)
        {
            _charged = true;
            _enemy.Attack();
        }

        if (_enemy.HasReachedDestionation())
        {
            _enemy.ChangeState<BoarCooldownState>();
            return;
        }
    }

    public override void Exit()
    {
        _enemy.Agent.speed = _enemy.BaseSpeed;
    }
}
