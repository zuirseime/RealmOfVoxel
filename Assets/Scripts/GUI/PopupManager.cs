using System.Linq;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private float _popUpLifetime;
    [SerializeField] private TextMeshProUGUI _critPopup;

    void Awake()
    {
        var playerInventory = FindObjectOfType<Inventory>();
        playerInventory.WeaponChanged += OnWeaponChanged;
    }

    private void OnWeaponChanged(object sender, WeaponEventArgs args)
    {
        args.Primary.CritStrike -= OnCritStrike;
        if (args.Secondary != null)
        {
            args.Secondary.CritStrike -= OnCritStrike;
        }
        args.Primary.CritStrike += OnCritStrike;
    }

    private void OnCritStrike(object sender, WeaponCritEventArgs args)
    {
        _critPopup.text = $"Crit! x{args.Multiplier}";
        var popup = Instantiate(_critPopup, transform);
        Destroy(popup, _popUpLifetime);
    }
}
