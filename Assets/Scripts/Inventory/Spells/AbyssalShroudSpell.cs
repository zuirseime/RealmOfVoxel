using UnityEngine;

[CreateAssetMenu(fileName = "AbyssalShroud", menuName = "Spells/Abyssal Shroud")]
public class AbyssalShroudSpell : Spell
{
    [SerializeField] private float _damage;
    [SerializeField] private float _tickRate;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _effectRadius;

    public override void Initialize()
    {
        base.Initialize();

        AddToStats("Damage", _damage / _tickRate, "/s");
        AddToStats("Range", _effectRadius);
        AddToStats("Mana", _manaCost);
        AddToStats("Duration", _duration, 's');
        AddToStats("Cooldown", _cooldown, 's');
    }

    protected override void ApplyEffect(Entity owner, Vector3 targetPosition)
    {
        var shroud = Instantiate(_effectPrefab, targetPosition, Quaternion.identity);

        var effect = shroud.GetComponent<AbyssalShroudEffect>();
        effect.Owner = owner;
        effect.Duration = _duration;
        effect.Radius = _range;
        effect.RotationSpeed = _rotationSpeed;
        effect.Damage = _damage;
        effect.TickRate = _tickRate;
        effect.EffectRadius = _effectRadius;
    }
}
