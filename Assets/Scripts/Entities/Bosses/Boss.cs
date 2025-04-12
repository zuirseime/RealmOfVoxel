using System;
using UnityEngine;

public abstract class Boss : Enemy
{
    [SerializeField, Range(0, 100)] protected int _secondPhaseThreshold;
    [SerializeField, ReadOnly] protected bool _isSecondPhase;
    [SerializeField] protected float _abilityCooldown;
    [field: SerializeField] public string Name { get; set; }

    public bool IsSecondPhase => _isSecondPhase;
    public float AbilityCooldown => _abilityCooldown;

    protected override void Awake()
    {
        base.Awake();
        _isSecondPhase = false;
        Health.ValueChanged += OnValueChanged;
    }

    protected override void Update()
    {
        base.Update();
        _currentState?.Update();
    }

    public bool ShouldEnterSecondPhase() 
        => !_isSecondPhase && Health.Value <= Health.MaxValue * _secondPhaseThreshold / 100f;

    protected virtual void EnterSecondPhase()
    {
        _isSecondPhase = true;
    }

    protected void OnValueChanged(object sender, AttributeEventArgs args)
    {
        if (ShouldEnterSecondPhase() && !_isSecondPhase)
        {
            EnterSecondPhase();
            Health.ValueChanged -= OnValueChanged;
        }
    }

    protected override void Die(Entity entity)
    {
        if (entity.TryGetComponent(out Inventory inventory))
        {
            inventory.AddCoins(Coins);
        }

        base.Die(entity);
        Destroy(gameObject);
    }
}
