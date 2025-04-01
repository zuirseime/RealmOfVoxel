using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Image _sprite;
    [SerializeField] private TextMeshProUGUI _priceTag;

    private bool _purcahsed = false;
    private Inventory _playersInventoryWithinReach;

    public Collectable Collectable { get; set; }

    public void SetCollectable(Collectable collectable)
    {
        Collectable = collectable;
        _priceTag.text = Collectable.Price.ToString();
        _sprite.sprite = Collectable.Sprite;
    }

    private void Start()
    {
        Game game = FindObjectOfType<Game>();
    }

    private void Update()
    {
        if (_playersInventoryWithinReach != null && !_purcahsed)
        {
            LetPlayerPurchase();
        }
    }

    private void LetPlayerPurchase()
    {
        if (Input.GetKeyDown(Settings.Instance.Input.Interact) 
            && _playersInventoryWithinReach.CanSpendCoins(Collectable.Price))
        {
            _purcahsed = true;
            _playersInventoryWithinReach.SpendCoins(Collectable.Price);

            if (Collectable is Weapon weapon)
            {
                _playersInventoryWithinReach.TakeWeapon(weapon);
            }
            else if (Collectable is Charm charm)
            {
                _playersInventoryWithinReach.EquipCharm(charm);
            }

            _sprite.sprite = null;
            _sprite.color = Color.clear;

            _playersInventoryWithinReach = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Inventory inventory) && !_purcahsed)
        {
            _playersInventoryWithinReach = inventory;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Inventory _))
        {
            _playersInventoryWithinReach = null;
        }
    }
}
