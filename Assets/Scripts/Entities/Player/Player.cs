using System;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private GameObject _selectionPrefab;

    public SelectionManager SelectionManager { get; private set; }
    public bool Original { get; set; } = true;
    public Inventory Inventory { get; private set; }

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

    protected override void Start()
    {
        base.Start();
        ChangeState(new PlayerIdleState(this));
        Inventory = GetComponent<Inventory>();
        SelectionManager = GetComponent<SelectionManager>();
    }

    protected override void Die()
    {
        base.Die();
        //Debug.Log("Player has died!");
        //agent.isStopped = true;
    }
}

