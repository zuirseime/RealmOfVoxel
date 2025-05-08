using UnityEngine;

public class WolfRetreatState : EnemyState<Wolf>
{
    private bool _retreated = false;

    public WolfRetreatState(Wolf wolf) : base(wolf) { }

    public override void Enter()
    {
        RefreshTimer(_enemy.RetreatDuration);
        _enemy.RetreatFromTarget();
    }

    public override void Update()
    {
        if (!_retreated && Time.time > _timer)
        {
            _retreated = true;
            RefreshTimer(_enemy.StunDuration);
        }

        if (_retreated && Time.time > _timer)
        {
            _enemy.ChangeState<WolfWanderState>();
        }
    }
}