using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellInventoryUI : MonoBehaviour
{
    [SerializeField] private List<SpellUIItem> _slots;
    [SerializeField] private MainMenuManager _mainMenuManager;

    private Game _game;

    private void Start()
    {
        _game = FindObjectOfType<Game>();
        if (_game == null)
        {
            Debug.LogError("Game instance not found.");
            return;
        }

        InitializeSlots();
        RefreshInventory();

        _game.SpellSetChanged += OnSpellSetChanged;
    }

    private void OnDestroy()
    {
        if (_game != null)
        {
            _game.SpellSetChanged -= OnSpellSetChanged;
        }

        foreach (var slot in _slots)
        {
            slot.OnItemBeginDrag -= _mainMenuManager.HandleDragStart;
            slot.OnItemEndDrag -= _mainMenuManager.HandleDragEnd;
            slot.OnItemDroppedOnSlot -= HandleShopItemDrop;
        }
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] != null)
            {
                _slots[i].InventorySlotIndex = i;
                _slots[i].IsInShop = false;

                _slots[i].OnItemBeginDrag += _mainMenuManager.HandleDragStart;
                _slots[i].OnItemEndDrag += _mainMenuManager.HandleDragEnd;
                _slots[i].OnItemDroppedOnSlot += HandleShopItemDrop;
            } else
            {
                Debug.LogWarning($"Inventory slot at index {i} is not assigned in the inspector!");
            }
        }
    }

    private void OnSpellSetChanged(object sender, SpellSetEventArgs e)
    {
        RefreshInventory();
        _mainMenuManager?.RefreshShopIU();
    }

    public void RefreshInventory()
    {
        Debug.Log("Refreshing inventory...");
        if (_game == null)
            return;

        Spell[] activeSpells = _game.CurrentSpellSet.ToArray();

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] == null)
                continue;

            if (i < activeSpells.Length && activeSpells[i] != null)
            {
                Spell spell = activeSpells[i];
                SpellData spellData = _game.SpellManager.GetSpellData(spell.ID);
                _slots[i].Initialize(spell, spellData, false);
            } else
            {
                _slots[i].Initialize(null, null, false);
            }
        }
    }

    private void HandleShopItemDrop(SpellUIItem draggedItem, SpellUIItem dropTargetSlot)
    {
        if (draggedItem.IsInShop && !dropTargetSlot.IsInShop)
        {
            Debug.Log($"Trying to replace spell in slot {dropTargetSlot.InventorySlotIndex} with {draggedItem.Spell.Title}");
            _game.ReplaceSpell(draggedItem.Spell.ID, dropTargetSlot.InventorySlotIndex);

            RefreshInventory();
            _mainMenuManager?.RefreshShopIU();
        } else
        {
            Debug.Log("Invalid drop: Cannot drop inventory item onto inventory or shop item onto shop.");
        }
    }
}
