using UnityEngine;

public class WolfAttackState : EnemyState<Wolf>
{
    public WolfAttackState(Wolf wolf) : base(wolf) { }

    public override void Enter()
    {
        _enemy.ClearDestination();
        _enemy.Target.Died += _enemy.OnTargetDied;
    }

    public override void Update()
    {
        if (Time.time < _timer)
            return;

        if (_enemy.Target != null)
            _enemy.transform.LookAt(_enemy.Target.transform);

        _enemy.Attack();
        _enemy.ChangeState<WolfRetreatState>();
    }

    public override void Exit()
    {
        if (_enemy.Target != null)
            _enemy.Target.Died -= _enemy.OnTargetDied;
        RefreshTimer(_enemy.AttackCooldown);
    }
}
