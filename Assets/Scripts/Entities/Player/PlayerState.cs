using System;
using UnityEngine;

[Serializable]
public abstract class PlayerState : EntityState
{
    protected Player _player;

    protected PlayerState(Player player)
    {
        _player = player;
    }
}

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

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

    public override void Update()
    {
        if (_player.HasReachedDestination())
        {
            _player.ChangeState(new PlayerIdleState(_player));
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 destination = hit.point;

                if (hit.collider.TryGetComponent(out Enemy enemy) && enemy.enabled)
                {
                    _player.SelectionManager.Select(enemy);
                    _player.ChangeState(new PlayerChaseState(_player));
                } else
                {
                    _player.SelectionManager.Select(destination);
                    _player.SetDestination(destination);
                }
            }
        }
    }
}

public class PlayerChaseState : PlayerState
{
    public PlayerChaseState(Player player) : base(player) { }

    public override void Enter()
    {
        _player.Agent.stoppingDistance = _player.Inventory.CurrentWeapon.Range;
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.ChangeState(new PlayerMoveState(_player));
            return;
        }

        if (_player.target == null)
        {
            _player.ChangeState(new PlayerIdleState(_player));
            return;
        }

        if (_player.CanAttackEnemy())
        {
            _player.ChangeState(new PlayerAttackState(_player));
            return;
        }

        _player.SetDestination(_player.target.transform.position);
    }

    public override void Exit()
    {
        _player.Agent.stoppingDistance = 0;
    }
}

public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(Player player) : base(player) { }

    public override void Enter()
    {
        _player.ClearDestination();
        _player.target.Died += _player.OnTargetDied;
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.ChangeState(new PlayerMoveState(_player));
            return;
        }

        if (_player.target == null)
        {
            _player.ChangeState(new PlayerIdleState(_player));
            return;
        }

        if (!_player.CanAttackEnemy())
        {
            _player.ChangeState(new PlayerChaseState(_player));
            return;
        }

        _player.transform.LookAt(_player.target.transform);
        _player.Attack();
    }

    public override void Exit()
    {
        if (_player.target != null)
            _player.target.Died -= _player.OnTargetDied;
    }
}

public class PlayerCastingState : PlayerState
{
    private Spell _spell;

    public PlayerCastingState(Player player, Spell spell) : base(player)
    {
        _spell = spell;
    }

    public override void Enter()
    {
        if (_spell == null)
        {
            _player.ChangeState(new PlayerIdleState(_player));
        } else
        {
            _spell.SpellDeselected += OnSpellDeselected;
        }
    }

    public override void Update()
    {
        if (_player.Mana.CanDrain(_spell.ManaCost))
        {
            _spell.Select();
            if (_spell.Range == 0)
            {
                _spell.CastAt(_player, Vector3.zero);
                _player.ChangeState(new PlayerMoveState(_player));
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (Vector3.Distance(_player.transform.position, hit.point) <= _spell.Range)
                        _spell.CastAt(_player, new Vector3(hit.point.x, 0, hit.point.z));
                }

                _player.ChangeState(new PlayerMoveState(_player));
            }
        } else
        {
            _player.ChangeState(new PlayerMoveState(_player));
        }
    }

    public override void Exit()
    {
        _spell?.Deselect();
    }

    private void OnSpellDeselected(object sender, SpellEventArgs args)
    {
        _spell.SpellDeselected -= OnSpellDeselected;
        _player.ChangeState(new PlayerMoveState(_player));
    }
}
