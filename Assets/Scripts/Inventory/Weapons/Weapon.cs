using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] protected string _name;

    [Header("Weapon Stats")]
    [SerializeField, Min(0)] protected float _baseDamage;
    [SerializeField, Min(0)] protected float _range;
    [SerializeField, Min(0)] protected float _cooldown;
    [SerializeField, Range(0f, 1f)] protected float _critChance;
    [SerializeField, Min(1f)] protected float _critMultiplier;

    protected float _nextAttackTime = 0;
    protected Animator _animator;

    public Sprite Sprite => _sprite;
    public string Name => _name;
    public float Damage { get; private set; }
    public float Range => _range;
    public float Cooldown => _cooldown;
    public float CritChance => _critChance;
    public float CritMultiplier => _critMultiplier;

    public event System.EventHandler<WeaponCritEventArgs> CritStrike;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        Damage = _baseDamage;
    }

    public void Attack(Entity target)
    {
        if (!CanAttack(target))
            return;

        if (Time.time < _nextAttackTime)
            return;

        Damage = _baseDamage;
        if (Random.value < _critChance)
        {
            Damage *= _critMultiplier;
            CritStrike?.Invoke(this, new WeaponCritEventArgs(_critMultiplier, Damage));
        }

        AttackLogic(target);

        _nextAttackTime = Time.time + Cooldown;
    }

    public virtual bool CanAttack(Entity target)
    {
        return target != null;
    }

    protected abstract void AttackLogic(Entity target);

    public abstract void Use();
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