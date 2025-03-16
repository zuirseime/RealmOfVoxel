using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsDisplay : MonoBehaviour
{
    [SerializeField] private Image _primary;
    [SerializeField] private Image _seconday;

    void Awake()
    {
        var player = FindObjectOfType<Player>();
        player.WeaponChanged += OnWeaponChanged;
    }

    private void OnWeaponChanged(object sender, WeaponEventArgs args)
    {
        _primary.sprite = args.Current.Sprite;
        _seconday.sprite = args.Weapons.First(w => w != args.Current).Sprite;
    }
}
