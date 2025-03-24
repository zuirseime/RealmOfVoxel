using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsDisplay : MonoBehaviour
{
    [SerializeField] private Image _primary;
    [SerializeField] private Image _seconday;

    void Awake()
    {
        var playerInventory = FindObjectOfType<Inventory>();
        playerInventory.WeaponChanged += OnWeaponChanged;
    }

    private void OnWeaponChanged(object sender, WeaponEventArgs args)
    {
        _primary.sprite = args.Primary.Sprite;
        _seconday.sprite = args.Secondary.Sprite;
    }
}
