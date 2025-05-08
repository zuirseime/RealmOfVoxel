public class BoarChaseState : EnemyState<Boar>
{
    public BoarChaseState(Boar boar) : base(boar) { }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<BoarWanderState>();
            return;
        }

        _enemy.SetDestination(_enemy.Target.transform.forward);
        _enemy.ChangeState<BoarChargeState>();
    }
}
