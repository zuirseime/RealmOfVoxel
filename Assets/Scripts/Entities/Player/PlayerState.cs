using UnityEngine;

public abstract class PlayerState : IState
{
    protected Player _player;

    public PlayerState(Player player)
    {
        _player = player;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void Enter()
    {
        Debug.Log("Player is now idle.");
    }

    public override void Update()
    {
        if (Input.GetMouseButton(1))
        {
            _player.ChangeState(new PlayerMoveState(_player));
        }
    }
}

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player) { }

    public override void Enter()
    {
        Debug.Log("Player is moving.");
    }

    public override void Update()
    {
        if (_player.HasReachedDestination())
        {
            _player.ChangeState(new PlayerIdleState(_player));
        }

        if (_player.CanAttackEnemy())
        {
            _player.ChangeState(new PlayerAttackState(_player));
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 destination = hit.point;

                if (hit.collider.TryGetComponent(out Enemy enemy) && enemy.enabled)
                {
                    destination = enemy.transform.position;
                    _player.SelectionManager.Select(enemy);
                } else
                    _player.SelectionManager.Select(destination);

                _player.SetDestination(destination);
            }
        }
    }
}

public class PlayerAttackState : PlayerState
{
    protected float _nextAttackTime;

    public PlayerAttackState(Player player) : base(player) { }

    public override void Enter()
    {
        _player.agent.stoppingDistance = _player.CurrentWeapon.range;
        Debug.Log("Player is attacking.");
    }

    public override void Update()
    {
        if (!_player.CanAttackEnemy() || Input.GetMouseButtonDown(1))
        {
            _player.ChangeState(new PlayerMoveState(_player));
        }

        if (_player.target != null)
        {
            _player.transform.LookAt(_player.target.transform);
        }

        if (_player.CurrentSpell == null)
        {
            _player.Attack();
        } else
        {
            _player.AlternativeAttack();
            _player.CurrentSpell = null;
        }
    }

    public override void Exit()
    {
        _player.agent.stoppingDistance = 0;
    }
}