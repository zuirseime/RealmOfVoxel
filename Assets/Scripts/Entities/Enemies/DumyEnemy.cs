public class DumyEnemy : Enemy
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Die() => throw new System.NotImplementedException();
}
