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
            _player.ChangeState<PlayerMoveState>();
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
            _player.ChangeState<PlayerIdleState>();
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
                    _player.ChangeState<PlayerChaseState>();
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
            _player.ChangeState<PlayerMoveState>();
            return;
        }

        if (_player.Target == null)
        {
            _player.ChangeState<PlayerIdleState>();
            return;
        }

        if (_player.CanAttackEnemy())
        {
            _player.ChangeState<PlayerAttackState>();
            return;
        }

        _player.SetDestination(_player.Target.transform.position);
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
        _player.Target.Died += _player.OnTargetDied;
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.ChangeState<PlayerMoveState>();
            return;
        }

        if (_player.Target == null)
        {
            _player.ChangeState<PlayerIdleState>();
            return;
        }

        if (!_player.CanAttackEnemy())
        {
            _player.ChangeState<PlayerChaseState>();
            return;
        }

        _player.transform.LookAt(_player.Target.transform);
        _player.Attack();
    }

    public override void Exit()
    {
        if (_player.Target != null)
            _player.Target.Died -= _player.OnTargetDied;
    }
}

public class PlayerCastingState : PlayerState
{
    public PlayerCastingState(Player player) : base(player) { }

    public override void Enter()
    {
        if (_player.ActiveSpell == null)
        {
            _player.ChangeState<PlayerIdleState>();
        } else
        {
            _player.ActiveSpell.SpellDeselected += OnSpellDeselected;
            _player.ActiveSpell.SpellUsed += OnSpellUsed;
        }
    }

    public override void Update()
    {
        if (_player.Mana.CanDrain(_player.ActiveSpell.ManaCost))
        {
            _player.ActiveSpell.Select();
            if (_player.ActiveSpell.Range == 0)
            {
                _player.ActiveSpell.CastAt(_player, Vector3.zero);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (Vector3.Distance(_player.transform.position, hit.point) <= _player.ActiveSpell.Range)
                        _player.ActiveSpell.CastAt(_player, new Vector3(hit.point.x, 0, hit.point.z));
                }
            }
        } else
        {
            _player.ChangeState<PlayerMoveState>();
        }
    }

    public override void Exit()
    {
        _player.ActiveSpell?.Deselect();
    }

    private void OnSpellUsed(object sender, SpellEventArgs args)
    {
        _player.PlaySound(args.Spell.CastSound);
        _player.ActiveSpell.SpellUsed -= OnSpellUsed;
    }

    private void OnSpellDeselected(object sender, SpellEventArgs args)
    {
        _player.ActiveSpell.SpellDeselected -= OnSpellDeselected;
        _player.ChangeState<PlayerMoveState>();
    }
}
