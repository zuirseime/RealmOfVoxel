using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event EventHandler<WeaponEventArgs> WeaponChanged;

    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Weapon _primary;
    [SerializeField] private Weapon _secondary;

    [SerializeField, ReadOnly] private Spell[] _spells;
    private Settings _settings;
    private Spell _sellectedSpell;

    public Weapon CurrentWeapon => _primary;
    public Spell[] Spells => _spells;

    private void Awake()
    {
        _settings = Settings.Instance;

        _primary = Instantiate(_primary, _weaponHolder);
        _secondary = Instantiate(_secondary, _weaponHolder);
        _secondary.gameObject.SetActive(false);
    }

    private void Start()
    {
        _spells = Game.Instance.CurrentSpellSet;
        OnWeaponChanged();
        
        foreach (var spell in Spells)
        {
            if (spell == null) continue;
            spell.SpellSelected += (sender, args) => _sellectedSpell = args.Spell;
            spell.SpellDeselected += (sender, args) => _sellectedSpell = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(_settings.Input.SwapWeapons)) SwapWeapons();

        if (Input.GetKeyDown(_settings.Input.Spell1)) UseSpell(0);
        if (Input.GetKeyDown(_settings.Input.Spell2)) UseSpell(1);
        if (Input.GetKeyDown(_settings.Input.Spell3)) UseSpell(2);
        if (Input.GetKeyDown(_settings.Input.Spell4)) UseSpell(3);
    }

    public void TakeWeapon(Weapon weapon)
    {
        if (_secondary == null)
        {
            _secondary = Instantiate(weapon, _weaponHolder);
            _secondary.gameObject.SetActive(false);
        } else
        {
            Destroy(_primary.gameObject);
            _primary = Instantiate(weapon, _weaponHolder);
        }

        OnWeaponChanged();
    }

    private void SwapWeapons()
    {
        if (_secondary == null)
            return;

        (_primary, _secondary) = (_secondary, _primary);

        _primary.gameObject.SetActive(true);
        _secondary.gameObject.SetActive(false);

        OnWeaponChanged();
    }

    public void UseSpell(int index)
    {
        if (index < 0 || index >= Spells.Length)
            return;

        if (_sellectedSpell != null && Spells[index].SpellName == _sellectedSpell.SpellName)
        {
            _sellectedSpell.Deselect();
            return;
        }

        GetComponent<Player>().ChangeState(new PlayerCastingState(GetComponent<Player>(), Spells[index]));
    }

    private void OnWeaponChanged()
    {
        WeaponChanged?.Invoke(this, new WeaponEventArgs(_primary, _secondary));
    }
}

public class WeaponEventArgs : EventArgs
{
    public Weapon Primary { get; private set; }
    public Weapon Secondary { get; private set; }

    public WeaponEventArgs(Weapon primary, Weapon secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }
}