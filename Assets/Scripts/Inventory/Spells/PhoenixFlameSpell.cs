using UnityEngine;

[CreateAssetMenu(fileName = "PhoenixFlame", menuName = "Spells/Phoenix Flame")]
public class PhoenixFlameSpell : AreaSpell
{
    [SerializeField] private float _heal;
    [SerializeField] private float _rotationSpeed;

    public override void Initialize()
    {
        base.Initialize();

        AddToStats("Heal", _heal);
        AddToStats("Range", _range);
        AddToStats("Mana", _manaCost);
        AddToStats("Radius", _maxExpansion);
        AddToStats("Duration", _duration, 's');
        AddToStats("Cooldown", _cooldown, 's');
    }

    protected override void ApplyEffect(Entity owner, Vector3 targetPosition)
    {
        Vector3 spawnPosition = targetPosition + Vector3.up * 0.2f;
        var aura = Instantiate(_effectPrefab, spawnPosition, Quaternion.identity);

        var effect = aura.GetComponent<PhoenixFlameEffect>();

        effect.Owner = owner;
        effect.Heal = _heal;
        effect.Duration = _duration;
        effect.Radius = _range;
        effect.MaxExpansion = _maxExpansion;
        effect.TickRate = _tickRate;
        effect.RotationSpeed = _rotationSpeed;
    }
}