using UnityEngine;

[CreateAssetMenu(fileName = "FrailityDome", menuName = "Spells/Dome of Fraility")]
public class FrailityDomeSpell : AreaSpell
{
    [SerializeField, Range(1, 100)] private float _protectionReduction;

    public override void Initialize()
    {
        base.Initialize();

        AddToStats("Protection", GetAbsoluteProtectionReduction(), '%');
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

        var effect = aura.GetComponent<FrailityDomeEffect>();
        effect.Owner = owner;
        effect.ProtectionReduction = GetAbsoluteProtectionReduction();
        effect.Duration = _duration;
        effect.Radius = _range;
        effect.MaxExpansion = _maxExpansion;
        effect.TickRate = _tickRate;
    }

    private float GetAbsoluteProtectionReduction()
    {
        return -(_protectionReduction / 100f);
    }
}