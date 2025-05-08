using UnityEngine;

public class Boar : Enemy
{
    [Header("Boar Specific")]
    [SerializeField] private float _chargeSpeed;
    [SerializeField] private float _chargeCooldown;
    [SerializeField] private float _chargeDistance;

    public float ChargeCooldown => _chargeCooldown;
    public float ChargeSpeed => _chargeSpeed;
    public float ChargeDistance => _chargeDistance;

    public override void Activate()
    {
        base.Activate();
        ChangeState<BoarWanderState>();
    }

    protected override void Awake()
    {
        base.Awake();
        _states = new EntityState[] {
            new BoarWanderState(this),
            new BoarChaseState(this),
            new BoarChargeState(this),
            new BoarCooldownState(this)
        };
    }
}
