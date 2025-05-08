public class MoleChaseState : EnemyState<Mole>
{
    public MoleChaseState(Mole mole) : base(mole) { }

    public override void Enter()
    {
        _enemy.SetModelActive(false);
    }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<MoleWanderState>();
            return;
        }

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<MoleAttackState>();
            return;
        }

        _enemy.SetDestination(_enemy.Target.transform.position);
    }
}
