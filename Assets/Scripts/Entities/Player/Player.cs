using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Inventory), typeof(WeaponEquipper))]
[RequireComponent(typeof(CharmEquipper), typeof(SpellCaster))]
[RequireComponent(typeof(Wallet), typeof(SelectionManager))]
public class Player : Entity
{
    [SerializeField] private GameObject _selectionPrefab;
    [SerializeField] private float _battleSpeed;
    [SerializeField] private float _sprintSpeed;

    private float _currentModeSpeed;

    public SelectionManager SelectionManager { get; private set; }
    public bool Original { get; set; } = true;
    public Inventory Inventory { get; private set; }
    public Spell ActiveSpell { get; private set; }

    public void SetActiveSpell(Spell spell)
    {
        ActiveSpell = spell;
        ActiveSpell.SpellUsed += OnSpellUsed;
    }

    public void SwitchToBattleMode()
    {
        _currentModeSpeed = _battleSpeed;
    }

    public void SwitchToSprintMode()
    {
        _currentModeSpeed = _sprintSpeed;
    }

    public override void Attack()
    {
        Inventory.CurrentWeapon.Attack(Target);
    }

    public bool HasReachedDestination()
    {
        return !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance;
    }

    public bool CanAttackEnemy()
    {
        return Target != null
            && transform.parent == Target.transform.parent
            && !Physics.Linecast(transform.position, Target.transform.position, LayerMask.GetMask("Level"))
            && Vector3.Distance(transform.position, Target.transform.position) <= Inventory.CurrentWeapon.Range;
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[]
        {
            new PlayerIdleState(this),
            new PlayerMoveState(this),
            new PlayerChaseState(this),
            new PlayerAttackState(this),
            new PlayerCastingState(this),
        };
    }

    private void Start()
    {
        ChangeState<PlayerIdleState>();
        SwitchToSprintMode();

        Inventory = GetComponent<Inventory>();
        SelectionManager = GetComponent<SelectionManager>();
    }

    protected override void Update()
    {
        base.Update();

        Agent.speed = _currentModeSpeed * GetComponent<EntityModifiers>().MoveSpeedModifier.Value;
    }

    protected override void Die(Entity entity)
    {
        base.Die(entity);

        Wallet wallet = GetComponent<Wallet>();

        MoneyManager.ConvertCoinsToMoney(Mathf.RoundToInt(wallet.Coins));
        SceneManager.LoadScene("MainMenu");
    }

    protected override void OnTargetChanged(object sender, Entity target)
    {
        base.OnTargetChanged(sender, target);
        ChangeState<PlayerChaseState>();
    }

    private void OnSpellUsed(object sender, SpellEventArgs args)
    {
        if (args.Spell == null)
            return;
        args.Spell.SpellUsed -= OnSpellUsed;
        ActiveSpell = null;
    }
}

