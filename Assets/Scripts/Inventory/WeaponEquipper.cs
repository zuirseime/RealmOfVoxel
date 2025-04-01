using System;
using UnityEngine;

public class WeaponEquipper : MonoBehaviour
{
    public event EventHandler<WeaponEventArgs> WeaponChanged;

    [SerializeField] private Transform _container;
    [SerializeField] private Weapon _primary;
    [SerializeField] private Weapon _secondary;

    public Weapon CurrentWeapon
    {
        get => _primary;
        private set => _primary = value;
    }
    public Weapon SecondaryWeapon
    {
        get => _secondary;
        private set => _secondary = value;
    }

    public void Equip(Weapon weapon)
    {
        if (SecondaryWeapon == null && CurrentWeapon != null)
        {
            SecondaryWeapon = Instantiate(weapon, _container);
            SecondaryWeapon.gameObject.SetActive(false);
        } else
        {
            if (CurrentWeapon != null)
            {
                Instantiate(CurrentWeapon, transform.position + Vector3.up, Quaternion.identity);
                Destroy(CurrentWeapon.gameObject);
            } 

            CurrentWeapon = Instantiate(weapon, _container);
        }

        OnWeaponChanged();
    }

    public void Swap()
    {
        if (SecondaryWeapon == null)
            return;

        (CurrentWeapon, SecondaryWeapon) = (SecondaryWeapon, CurrentWeapon);

        CurrentWeapon?.gameObject.SetActive(true);
        SecondaryWeapon?.gameObject.SetActive(false);

        OnWeaponChanged();
    }

    private void Start()
    {
        if (CurrentWeapon == null)
            Equip(FindObjectOfType<Game>().GetStartWeapon());
        else
            Equip(CurrentWeapon);

        if (SecondaryWeapon != null)
            Equip(SecondaryWeapon);

        OnWeaponChanged();
    }

    private void OnWeaponChanged()
    {
        WeaponChanged?.Invoke(this, new WeaponEventArgs(CurrentWeapon, SecondaryWeapon));
    }
}
