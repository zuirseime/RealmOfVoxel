using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public event EventHandler<Weapon> ChestOpened;

    private SphereCollider _sphereCollider;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TextMeshProUGUI _hint;

    [field:SerializeField] public Weapon StoredWeapon { get; set; }
    public int StoredCoins { get; set; }

    private bool _opened = false;

    private Inventory _playersInventoryWithinReach;

    void Start()
    {
        _hint.text = Settings.Instance.Input.Interact.ToString();
        _sphereCollider = GetComponent<SphereCollider>();
        _canvas.enabled = false;
    }

    private void Update()
    {
        if (_playersInventoryWithinReach != null)
        {
            LetPlayerOpenChest();
        }
    }

    private void LetPlayerOpenChest()
    {
        if (Input.GetKeyDown(Settings.Instance.Input.Interact))
        {
            Instantiate(StoredWeapon, transform.position + Vector3.up * 2, Quaternion.identity);
            _playersInventoryWithinReach.AddCoins(StoredCoins);

            _playersInventoryWithinReach = null;
            _sphereCollider.enabled = false;
            _canvas.enabled = false;

            ChestOpened?.Invoke(this, StoredWeapon);
            StoredWeapon = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WeaponEquipper playersInventory) && !_opened)
        {
            _playersInventoryWithinReach = playersInventory.GetComponent<Inventory>();
            _canvas.enabled = true;

            List<Weapon> heldWeapons = new();
            if (playersInventory.CurrentWeapon != null)
                heldWeapons.Add(playersInventory.CurrentWeapon);
            if (playersInventory.SecondaryWeapon != null)
                heldWeapons.Add(playersInventory.SecondaryWeapon);

            StoredWeapon = Game.Instance.GetWeapon(heldWeapons.ToArray());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Inventory _))
        {
            _playersInventoryWithinReach = null;
            _canvas.enabled = false;
        }
    }
}
