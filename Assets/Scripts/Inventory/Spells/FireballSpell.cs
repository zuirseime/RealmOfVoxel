using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "Spells/Fireball")]
public class FireballSpell : Spell
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private float _maxExpansion;

    public override void Initialize()
    {
        base.Initialize();

        AddToStats("Damage", _damage);
        AddToStats("Range", _range);
        AddToStats("Mana", _manaCost);
        AddToStats("Radius", _maxExpansion);
        AddToStats("Duration", _duration, 's');
        AddToStats("Cooldown", _cooldown, 's');
    }

    protected override void ApplyEffect(Entity owner, Vector3 targetPosition)
    {
        targetPosition += Vector3.up;

        Vector3 spawnPosition = owner.transform.position + Vector3.up;
        var projectile = Instantiate(_effectPrefab, spawnPosition, Quaternion.LookRotation((targetPosition - spawnPosition).normalized));
        projectile.transform.LookAt(targetPosition);

        var effect = projectile.GetComponent<FireballEffect>();
        effect.Owner = owner;
        effect.Damage = _damage;
        effect.Duration = _duration;
        effect.Radius = _range;
        effect.MoveSpeed = _moveSpeed;
        effect.MaxExpansion = _maxExpansion;
        effect.SetTarget(targetPosition);
    }
}
