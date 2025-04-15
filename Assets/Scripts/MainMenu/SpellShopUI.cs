using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellShopUI : MonoBehaviour
{
    [SerializeField] private SpellUIItem _spellUIPrefab;
    [SerializeField] private Transform _shopContainer;
    [SerializeField] private MainMenuManager _mainMenuManager;

    private List<SpellUIItem> _shopItems = new();

    private void Start()
    {
        Game game = FindObjectOfType<Game>();

        if (game == null || game.SpellManager == null)
        {
            Debug.Log("Game instance or SpellManager not found.");
            return;
        }
        PopulateShop(game);

        game.SpellSetChanged += OnSpellSetChanged;
    }

    private void OnDestroy()
    {
        if (Game.Instance != null)
        {
            Game.Instance.SpellSetChanged -= OnSpellSetChanged;
        }
    }

    private void PopulateShop(Game game)
    {
        foreach (Transform child in _shopContainer)
        {
            Destroy(child.gameObject);
        }
        _shopItems.Clear();

        Spell[] allSpells = game.SpellManager.GetAll();

        if (allSpells == null)
        {
            Debug.LogError("No spells found in the SpellManager.");
            return;
        }

        foreach (Spell spell in allSpells)
        {
            SpellUIItem spellUI = Instantiate(_spellUIPrefab, _shopContainer);

            if (spellUI != null)
            {
                SpellData data = game.SpellManager.GetSpellData(spell.ID);
                spellUI.Initialize(spell, data, true);
                spellUI.OnItemClicked += HandleSpellPurchaseAttempt;
                spellUI.OnItemBeginDrag += _mainMenuManager.HandleDragStart;
                spellUI.OnItemEndDrag += _mainMenuManager.HandleDragEnd;
                _shopItems.Add(spellUI);
            } else
            {
                Debug.LogError("SpellUIItem component not found on prefab.");
                Destroy(spellUI.gameObject);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_shopContainer as RectTransform);
    }

    private void HandleSpellPurchaseAttempt(SpellUIItem item)
    {
        if (item.SpellData != null && item.SpellData.Acquired)
        {
            Debug.Log($"Spell '{item.Spell.Title}' is already acquired.");
            return;
        }

        if (MoneyManager.SpendMoney(Mathf.RoundToInt(item.Spell.Price)))
        {
            Debug.Log($"Purchased '{item.Spell.Title}'");
            Game game = FindObjectOfType<Game>();
            game.AcquireSpell(item.Spell.ID);

            SpellData updatedData = game.SpellManager.GetSpellData(item.Spell.ID);
            item.UpdateData(updatedData);
        } else
        {
            Debug.Log($"Not enough money to purchase '{item.Spell.Title}'. Need {item.Spell.Price}, have {MoneyManager.CurrentMoney}");
        }
    }

    private void OnSpellSetChanged(object sender, SpellSetEventArgs e)
    {
        RefreshShopVisuals();
    }

    public void RefreshShopVisuals()
    {
        Debug.Log("Refreshing Shop Visuals");
        foreach (var item in _shopItems)
        {
            SpellData data = FindObjectOfType<Game>().SpellManager.GetSpellData(item.Spell.ID);
            if (data != null)
            {
                item.UpdateData(data);
            }
        }
    }
}