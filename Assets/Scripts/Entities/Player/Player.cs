using System;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private GameObject _selectionPrefab;
    [SerializeField] private float _battleSpeed;
    [SerializeField] private float _sprintSpeed;

    private float _currentModeSpeed;

    public SelectionManager SelectionManager { get; private set; }
    public bool Original { get; set; } = true;
    public Inventory Inventory { get; private set; }

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
        Inventory.CurrentWeapon.Attack(target);
    }

    public bool HasReachedDestination()
    {
        return !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance;
    }

    public bool CanAttackEnemy()
    {
        return target != null 
            && transform.parent == target.transform.parent
            && !Physics.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Level")) 
            && Vector3.Distance(transform.position, target.transform.position) <= Inventory.CurrentWeapon.Range;
    }

    private void Start()
    {
        ChangeState(new PlayerIdleState(this));
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
        //Debug.Log("Player has died!");
        //agent.isStopped = true;
    }
}

