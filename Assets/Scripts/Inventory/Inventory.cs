using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(WeaponEquipper), typeof(CharmEquipper))]
[RequireComponent(typeof(SpellCaster), typeof(Wallet))]
public class Inventory : MonoBehaviour
{
    public event EventHandler<WeaponEventArgs> WeaponChanged;
    public event EventHandler<CoinsEventArgs> CoinsChanged;
    public event EventHandler<Charm> CharmChanged;
    public event EventHandler<Spell[]> SpellsChanged;

    private WeaponEquipper _weaponEquipper;
    private CharmEquipper _charmEquipper;
    private SpellCaster _spellCaster;
    private Wallet _wallet;

    private Settings _settings;
    [SerializeField] private float _collectingCooldown;
    private float _nextCollecting;

    public Weapon CurrentWeapon => _weaponEquipper.CurrentWeapon;
    public Weapon SecondayWeapon => _weaponEquipper.SecondaryWeapon;
    public Spell[] Spells => _spellCaster.Spells;
    public float LocalCoins => _wallet.Coins;

    public bool CanCollect() => Time.time >= _nextCollecting;

    public void TakeWeapon(Weapon weapon)
    {
        if (!CanCollect()) return;

        _weaponEquipper.Equip(weapon);
        _nextCollecting = Time.time + _collectingCooldown;
    }

    public void EquipCharm(Charm newCharm)
    {
        if (!CanCollect()) return;

        _charmEquipper.Equip(newCharm);
        _nextCollecting = Time.time + _collectingCooldown;
    }

    public bool CanSpendCoins(int amount)
    {
        return _wallet.CanSpend(amount);
    }

    public void AddCoins(int amount)
    {
        _wallet.Add(amount * GetComponent<EntityModifiers>().CoinModifier.Value);
    }

    public void SpendCoins(int amount)
    {
        _wallet.Spend(amount);
    }

    private void Awake()
    {
        _settings = Settings.Instance;
        _weaponEquipper = GetComponent<WeaponEquipper>();
        _charmEquipper = GetComponent<CharmEquipper>();
        _spellCaster = GetComponent<SpellCaster>();
        _wallet = GetComponent<Wallet>();

        _weaponEquipper.WeaponChanged += (sender, args) => WeaponChanged?.Invoke(this, args);
        _charmEquipper.CharmChanged += (sender, charm) => CharmChanged?.Invoke(this, charm);
        _wallet.CoinsChanged += (sender, args) => CoinsChanged?.Invoke(this, args);
        _spellCaster.SpellsChanged += (sender, spells) => SpellsChanged?.Invoke(this, spells);
    }

    private void OnEnable()
    {
        FindObjectOfType<Game>().SpellSetChanged += OnSpellSetChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_settings.Input.SwapWeapons)) _weaponEquipper.Swap();

        if (Input.GetKeyDown(_settings.Input.Spell1)) _spellCaster.UseSpell(0);
        if (Input.GetKeyDown(_settings.Input.Spell2)) _spellCaster.UseSpell(1);
        if (Input.GetKeyDown(_settings.Input.Spell3)) _spellCaster.UseSpell(2);
        if (Input.GetKeyDown(_settings.Input.Spell4))
            _spellCaster.UseSpell(3);
    }

    private void OnSpellSetChanged(object sender, SpellSetEventArgs args)
    {
        _spellCaster.SetSpells(args.Spells);
    }
}
