using UnityEngine;

[CreateAssetMenu(fileName = "Shield", menuName = "Spells/Shield")]
public class ShieldSpell : Spell
{
    [SerializeField] private float _rotationSpeed;
    [Tooltip("Percentage of damage absorbed by the Divine Shield")]
    [SerializeField] private float _absorbtion;

    public override void Initialize()
    {
        base.Initialize();

        AddToStats("Mana", _manaCost);
        AddToStats("Absortion", _absorbtion, '%');
        AddToStats("Duration", _duration, 's');
        AddToStats("Cooldown", _cooldown, 's');
    }

    protected override void ApplyEffect(Entity owner, Vector3 targetPosition)
    {
        var shield = Instantiate(_effectPrefab, targetPosition, Quaternion.identity);

        var effect = shield.GetComponent<ShieldEffect>();
        effect.Owner = owner;
        effect.Duration = _duration;
        effect.Radius = _range;
        effect.RotationSpeed = _rotationSpeed;
        effect.Absorbtion = _absorbtion;
    }
}
