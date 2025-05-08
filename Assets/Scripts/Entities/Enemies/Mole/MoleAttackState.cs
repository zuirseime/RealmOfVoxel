using UnityEngine;

public class MoleAttackState : EnemyState<Mole>
{
    private float _onLandTimer;

    public MoleAttackState(Mole mole) : base(mole) { }

    public override void Enter()
    {
        _onLandTimer = Time.time + _enemy.LandDuration;
        _enemy.ClearDestination();
        _enemy.Target.Died += _enemy.OnTargetDied;

        _enemy.StartAttack();
    }

    public override void Update()
    {
        if (_enemy.Target == null)
        {
            _enemy.ChangeState<MoleWanderState>();
            return;
        }

        if (!_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<MoleChaseState>();
            return;
        }

        _enemy.transform.LookAt(_enemy.Target.transform);

        if (Time.time < _timer)
            return;

        _enemy.Attack();

        RefreshTimer(_enemy.AttackCooldown);

        if (Time.time >= _onLandTimer)
        {
            _enemy.Retreat();
            _enemy.ChangeState<MoleWanderState>();
        }
    }

    public override void Exit()
    {
        _enemy.Target.Died -= _enemy.OnTargetDied;
        _enemy.EndAttack();
    }
}