using UnityEngine;
using UnityEngine.AI;

public class MoleWanderState : EnemyWanderState<Mole>
{
    public MoleWanderState(Mole mole) : base(mole) { }

    public override void Enter()
    {
        _enemy.ClearDestination();
        Wander();
        _enemy.SetModelActive(false);
    }

    public override void Update()
    {
        if (!_enemy.Retreating && _enemy.HasPlayerInDetectionRange())
        {
            _enemy.ChangeState<MoleChaseState>();
            return;
        }

        if (Time.time < _timer)
            return;

        Wander();
    }
}
