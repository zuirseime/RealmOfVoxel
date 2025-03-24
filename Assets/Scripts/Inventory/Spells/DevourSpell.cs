using UnityEngine;

[CreateAssetMenu(fileName = "Devour", menuName = "Spells/Devour")]
public class DevourSpell : AreaSpell
{
    [SerializeField] private float _damage;

    public override void Initialize()
    {
        base.Initialize();

        AddToStats("Damage", _damage / _tickRate, "/s");
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

        var effect = aura.GetComponent<DevourEffect>();
        effect.Owner = owner;
        effect.Damage = _damage;
        effect.Duration = _duration;
        effect.Radius = _range;
        effect.MaxExpansion = _maxExpansion;
        effect.TickRate = _tickRate;
    }
}
