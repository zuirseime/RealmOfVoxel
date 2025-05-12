using UnityEngine;

public abstract class Weapon : Collectable
{
    [Header("Weapon Stats")]
    [SerializeField, Min(0)] protected float _baseDamage = 1;
    [SerializeField, Min(0)] protected float _range = 1;
    [SerializeField, Min(0)] protected float _cooldown = 1;
    [SerializeField, Range(0f, 1f)] protected float _critChance = 1;
    [SerializeField, Min(1f)] protected float _critMultiplier = 2;

    [Header("Audo Settings")]
    [SerializeField] private AudioClip _attackSound;
    [SerializeField] private AudioSource _audioSource;

    protected float _nextAttackTime = 0;

    public float Damage { get; private set; }
    public float Range => _range;
    public float Cooldown => _cooldown * Owner.GetComponent<EntityModifiers>().CooldownModifier.Value;
    public float CritChance => _critChance * Owner.GetComponent<EntityModifiers>().CritChanceModifier.Value;
    public float CritMultiplier => _critMultiplier * Owner.GetComponent<EntityModifiers>().CritMultiplicationModifier.Value;
    public Entity Owner { get; set; }

    public event System.EventHandler<WeaponCritEventArgs> CritStrike;

    protected override void Awake()
    {
        base.Awake();

        Owner = FindObjectOfType<Player>();
    }

    protected override void Start()
    {
        base.Start();

        Damage = _baseDamage;

        AddToStats("Damage", Damage);
        AddToStats("Range", Range);
        AddToStats("Cooldown", Cooldown, 's');
        AddToStats("Critical Chance", CritChance * 100, '%');
    }

    public void Attack(Entity target)
    {
        if (!CanAttack(target))
            return;

        if (Time.time < _nextAttackTime)
            return;

        if (!target.CanBeDamagedBy(Owner))
            return;

        Damage = _baseDamage * Owner.GetComponent<EntityModifiers>().DamageModifier.Value;
        if (Random.value < CritChance)
        {
            Damage *= CritMultiplier;
            CritStrike?.Invoke(this, new WeaponCritEventArgs(CritMultiplier, Damage));
        }

        PlayAttackSound();
        AttackLogic(target);

        _nextAttackTime = Time.time + Cooldown;
    }

    private void PlayAttackSound()
    {
        if (_attackSound == null) return;

        if (_audioSource != null)
            _audioSource.PlayOneShot(_attackSound);
        else
            AudioSource.PlayClipAtPoint(_attackSound, transform.position);
    }

    public virtual bool CanAttack(Entity target)
    {
        return target != null;
    }

    public override void Collect(Player player)
    {
        if (!player.TryGetComponent(out Inventory inventory)) return;
        if (!inventory.CanCollect()) return;

        transform.position = Vector3.zero;
        inventory.TakeWeapon(this);

        base.Collect(player);

        Destroy(gameObject);
    }

    protected abstract void AttackLogic(Entity target);
}

public class WeaponCritEventArgs : System.EventArgs
{
    public float Multiplier { get; private set; }
    public float Damage { get; private set; }

    public WeaponCritEventArgs(float multiplier, float damage)
    {
        Multiplier = multiplier;
        Damage = damage;
    }
}
