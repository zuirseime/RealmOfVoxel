public class TreasurerChaseState : BossState
{
    public TreasurerChaseState(Treasurer boss) : base(boss) { }

    public override void Enter()
    {
        _boss.SetDestination(_boss.target.transform.position);
    }

    public override void Update()
    {
        if (_boss.target == null || !_boss.target.IsAlive)
        {
            _boss.ChangeState(new TreasurerWanderState((Treasurer)_boss));
            return;
        }

        _boss.SetDestination(_boss.target.transform.position);

        if (_boss.HasPlayerInAttackRange())
        {
            _boss.ChangeState(new TreasurerAttackState((Treasurer)_boss));
        } else if (!_boss.HasPlayerInDetectionRange())
        {
            _boss.ChangeState(new TreasurerWanderState((Treasurer)_boss));
        }
    }

    public override void Exit()
    {
        _boss.ClearDestination();
    }
}
