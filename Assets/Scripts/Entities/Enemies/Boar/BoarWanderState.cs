public class BoarWanderState : EnemyWanderState<Boar>
{
    public BoarWanderState(Boar boar) : base(boar) { }

    public override void Enter()
    {
        RefreshTimer(_enemy.WanderCooldown);
        _enemy.ClearDestination();
    }

    public override void Update()
    {
        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<BoarChaseState>();
            return;
        }

        if (!IsTimerFinished())
            return;

        Wander();
    }
}
