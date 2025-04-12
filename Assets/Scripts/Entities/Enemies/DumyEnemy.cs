public class DumyEnemy : Enemy
{
    public override void Activate()
    {
        base.Activate();
        ChangeState(new DummyEnemyWanderState(this));
    }

    protected override void Update()
    {
        base.Update();
    }
}
