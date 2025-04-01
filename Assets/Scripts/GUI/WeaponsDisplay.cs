using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hint;

    [SerializeField] private UISlot _primarySlot;
    [SerializeField] private UISlot _secondarySlot;

    void Awake()
    {
        var playerInventory = FindObjectOfType<Inventory>();
        playerInventory.WeaponChanged += OnWeaponChanged;
    }

    private void Start()
    {
        _hint.text = Settings.Instance.Input.SwapWeapons.ToString();
    }

    private void OnWeaponChanged(object sender, WeaponEventArgs args)
    {
        _primarySlot.Initialize(args.Primary);
        _secondarySlot.Initialize(args.Secondary);
    }
}
