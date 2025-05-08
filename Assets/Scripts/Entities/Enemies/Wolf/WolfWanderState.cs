using UnityEngine;
using UnityEngine.AI;

public class WolfWanderState : EnemyWanderState<Wolf>
{
    public WolfWanderState(Wolf wolf) : base(wolf) { }

    public override void Enter()
    {
        _enemy.ClearDestination();
        Wander();
    }

    public override void Update()
    {
        if (_enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<WolfFlankState>();
            return;
        }

        if (Time.time < _timer)
            return;

        Wander();
    }
}
