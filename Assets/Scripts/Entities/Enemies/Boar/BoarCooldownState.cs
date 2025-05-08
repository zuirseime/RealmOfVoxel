public class BoarCooldownState : EnemyState<Boar>
{
    public BoarCooldownState(Boar boar) : base(boar) { }

    public override void Enter()
    {
        _enemy.ClearDestination();
        RefreshTimer(_enemy.ChargeCooldown);
    }

    public override void Update()
    {
        if (!IsTimerFinished())
            return;

        _enemy.ChangeState<BoarWanderState>();
    }
}