public class TreasurerChaseState : BossState
{
    public TreasurerChaseState(Treasurer boss) : base(boss) { }

    public override void Enter()
    {
        _boss.SetDestination(_boss.Target.transform.position);
    }

    public override void Update()
    {
        if (_boss.Target == null || !_boss.Target.IsAlive)
        {
            _boss.ChangeState<TreasurerWanderState>();
            return;
        }

        _boss.SetDestination(_boss.Target.transform.position);

        if (_boss.HasPlayerInAttackRange())
        {
            _boss.ChangeState<TreasurerAttackState>();
        } else if (!_boss.HasPlayerInDetectionRange())
        {
            _boss.ChangeState<TreasurerWanderState>();
        }
    }

    public override void Exit()
    {
        _boss.ClearDestination();
    }
}
