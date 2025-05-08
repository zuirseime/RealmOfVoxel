public class TreasurerChaseState : EnemyState<Treasurer>
{
    public TreasurerChaseState(Treasurer treasurer) : base(treasurer) { }

    public override void Enter()
    {
        _enemy.SetDestination(_enemy.Target.transform.position);
    }

    public override void Update()
    {
        if (_enemy.Target == null || !_enemy.Target.IsAlive)
        {
            _enemy.ChangeState<TreasurerWanderState>();
            return;
        }

        _enemy.SetDestination(_enemy.Target.transform.position);

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<TreasurerAttackState>();
        } else if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<TreasurerWanderState>();
        }
    }

    public override void Exit()
    {
        _enemy.ClearDestination();
    }
}
