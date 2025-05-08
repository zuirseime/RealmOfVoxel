using UnityEngine;
using UnityEngine.AI;

public class Mole : Enemy
{
    [Header("Mole Specific")]
    [SerializeField] private Renderer _model;
    [SerializeField] private Collider _collider;
    [SerializeField] private Canvas _info;
    [SerializeField] private float _landDuration = 0.25f;
    [SerializeField] private float _retreatingCooldown = 2f;

    private bool _isAttacking = false;
    private float _retreatTimer;
    private bool _isRetreating = false;

    public float LandDuration => _landDuration;
    public bool IsAttacking => _isAttacking;
    public bool Retreating => _isRetreating && _retreatTimer > Time.time;
    public void Retreat()
    {
        _isRetreating = true;
        _retreatTimer = Time.time + _retreatingCooldown;
    }

    public override bool CanBeDamagedBy(Entity attacker) => _isAttacking;

    public void StartAttack()
    {
        _isAttacking = true;
        SetModelActive(true);
    }

    public void EndAttack()
    {
        _isAttacking = false;
        SetModelActive(false);
    }

    public override void Activate()
    {
        base.Activate();
        ChangeState<MoleWanderState>();
    }

    public void SetModelActive(bool isActive)
    {
        _model.enabled = isActive;
        _collider.enabled = isActive;
        _info.enabled = isActive;
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[]
        {
            new MoleWanderState(this),
            new MoleChaseState(this),
            new MoleAttackState(this)
        };
    }
}
