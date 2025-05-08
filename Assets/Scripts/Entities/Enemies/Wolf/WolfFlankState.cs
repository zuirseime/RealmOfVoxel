public class WolfFlankState : EnemyState<Wolf>
{
    public WolfFlankState(Wolf wolf) : base(wolf) { }

    public override void Update()
    {
        if (!_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<WolfWanderState>();
            return;
        }

        if (_enemy.HasPlayerInAttackRange())
        {
            _enemy.ChangeState<WolfAttackState>();
            return;
        }

        _enemy.JumpTowardsTarget();
    }
}
