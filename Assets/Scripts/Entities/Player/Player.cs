using System;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private GameObject _selectionPrefab;
    [SerializeField] private Weapon[] _weapons = new Weapon[2];
    [SerializeField] private MonoBehaviour[] _spells = new MonoBehaviour[4];
    [field: SerializeField] public Weapon CurrentWeapon { get; private set; }
    [field: SerializeField] public MonoBehaviour CurrentSpell { get; set; }

    [SerializeField] private Transform _weaponHolder;
    private PlayerState _currentState;
    private int _currentWeaponIndex;

    public SelectionManager SelectionManager { get; private set; }
    public bool Original { get; set; } = true;

    public event EventHandler<WeaponEventArgs> WeaponChanged;

    public override void Attack()
    {
        CurrentWeapon.Attack(target);
    }

    public bool HasReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public void ShowSelection(Vector3 position)
    {
    }

    public void ChangeState(PlayerState playerState)
    {
        _currentState.Exit();
        _currentState = playerState;
        _currentState.Enter();
    }

    public bool CanAttackEnemy()
    {
        return target != null 
            && transform.parent == target.transform.parent
            && !Physics.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Level")) 
            && CurrentWeapon.CanAttack(target);
    }

    protected override void Awake()
    {
        base.Awake();

        ChangeWeapon(0);

        _currentState = new PlayerIdleState(this);
        SelectionManager = GetComponent<SelectionManager>();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CurrentSpell = _spells[0];
        } else if (Input.GetKeyDown(KeyCode.W))
        {
            CurrentSpell = _spells[1];
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            CurrentSpell = _spells[2];
        } else if (Input.GetKeyDown(KeyCode.R))
        {
            CurrentSpell = _spells[3];
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeWeapon((_currentWeaponIndex + 1) % _weapons.Length);
        }

        _currentState?.Update();
    }

    private void ChangeWeapon(int index)
    {
        if (index >= 0 && index < _weapons.Length && CurrentWeapon != _weapons[index])
        {
            WeaponChanged?.Invoke(this, new WeaponEventArgs(_weapons[index], _weapons));

            if (CurrentWeapon != null)
            {
                Destroy(CurrentWeapon.gameObject);
                CurrentWeapon = null;
            }
            CurrentWeapon = Instantiate(_weapons[index], _weaponHolder.position, _weaponHolder.rotation, _weaponHolder);
            _currentWeaponIndex = index;
        }
    }

    protected override void Die()
    {
        base.Die();
        //Debug.Log("Player has died!");
        //agent.isStopped = true;
    }
}

public class WeaponEventArgs : EventArgs
{
    public Weapon Current { get; private set; }
    public Weapon[] Weapons { get; private set; }

    public WeaponEventArgs(Weapon current, Weapon[] weapons)
    {
        Current = current;
        Weapons = weapons;
    }
}