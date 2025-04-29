using System;
using UnityEngine;

public abstract class Charm : Collectable
{
    [SerializeField, Range(0f, 100f)] protected int _bonus;
    protected float _savedValue;

    protected override void Start()
    {
        base.Start();

        AddToStats("Bonus", _bonus, '%');
    }

    public abstract void ApplyEffect(Entity entity);
    public abstract void RemoveEffect(Entity entity);

    protected float GetIncreaser() => (_bonus + 100) / 100f;
    protected float GetDecreaser() => (100 - _bonus) / 100f;

    public override void Collect(Player player)
    {
        if (!player.TryGetComponent(out Inventory inventory)) return;
        if (!inventory.CanCollect()) return;

        transform.position = Vector3.zero;
        inventory.EquipCharm(this);

        base.Collect(player);

        Destroy(gameObject);
    }
}
