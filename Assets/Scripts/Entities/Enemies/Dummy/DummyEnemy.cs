public class DummyEnemy : Enemy
{
    public override void Activate()
    {
        base.Activate();
        ChangeState<DummyEnemyWanderState>();
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[]
        {
            new DummyEnemyWanderState(this),
            new DummyEnemyChaseState(this),
            new DummyEnemyAttackState(this)
        };
    }

    protected override void Update()
    {
        base.Update();
    }
}
