using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _overlay;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] protected Tooltip _tooltip;

    public Spell Spell { get; private set; }
    public SpellData SpellData { get; private set; }
    public bool IsInShop { get; set; } = false;
    public int InventorySlotIndex { get; set; } = -1;

    public Action<SpellUIItem> OnItemClicked { get; set; }
    public Action<PointerEventData, SpellUIItem> OnItemBeginDrag { get; set; }
    public Action<PointerEventData> OnItemEndDrag { get; set; }
    public Action<SpellUIItem, SpellUIItem> OnItemDroppedOnSlot { get; set; }

    private bool _isDraggable = false;
    private static SpellUIItem _draggedItem = null;

    private void Start()
    {
        _tooltip = FindObjectOfType<MainMenuManager>().Tooltip;
    }

    public void Initialize(Spell spell, SpellData spellData, bool isInShop)
    {
        Spell = spell;
        SpellData = spellData;
        IsInShop = isInShop;

        if (spell != null)
        {
            _icon.sprite = spell.Sprite;
            _icon.enabled = true;
        } else
        {
            _icon.sprite = null;
            _icon.enabled = false;
        }

        UpdateVisualState();
    }

    public void UpdateData(SpellData spellData)
    {
        SpellData = spellData;
        UpdateVisualState();
    }

    public void UpdateVisualState()
    {
        if (SpellData == null)
        {
            _overlay.enabled = false;
            _canvasGroup.alpha = 1f;
            _isDraggable = false;
            return;
        }

        bool acquired = SpellData?.Acquired ?? false;
        bool active = SpellData?.Active ?? false;

        _overlay.enabled = IsInShop && !acquired;

        _canvasGroup.alpha = (IsInShop && !acquired) ? 0.25f : (IsInShop && active) ? 0.5f : 1f;

        _isDraggable = (IsInShop && acquired && !active) || (!IsInShop && acquired && active);

        _canvasGroup.blocksRaycasts = !(IsInShop && active);

        Debug.Log($"Spell: {Spell?.Title}, Acquired: {acquired}, Active: {active}, IsInShop: {IsInShop}, IsDraggable: {_isDraggable}, BlocksRaycasts: {_canvasGroup.blocksRaycasts}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsInShop && SpellData != null && !SpellData.Acquired)
        {
            Debug.Log($"Clicked to buy: {Spell.Title}");
            OnItemClicked?.Invoke(this);
        }
        else if (!IsInShop && SpellData != null && SpellData.Active)
        {
            Debug.Log($"Clicked on active inventory spell: {Spell.Title}");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDraggable || Spell == null || !IsInShop)
        {
            eventData.pointerDrag = null;
            return;
        }

        Debug.Log($"Begin drag: {Spell.Title}");
        _draggedItem = this;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;

        OnItemBeginDrag?.Invoke(eventData, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedItem == this)
        {
            FindObjectOfType<MainMenuManager>().Tooltip.gameObject.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedItem == this)
        {
            Debug.Log($"End drag: {Spell.Title}");
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;

            OnItemEndDrag?.Invoke(eventData);
            _draggedItem = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        SpellUIItem draggedItem = GetDraggedItem();

        if (!IsInShop && draggedItem != null && draggedItem.Spell != null)
        {
            if (draggedItem.IsInShop)
            {
                Debug.Log($"Dropped SHOP item {draggedItem.Spell.Title} onto inventory slot {InventorySlotIndex}");
                OnItemDroppedOnSlot?.Invoke(draggedItem, this);
            }
        } else
        {
            Debug.Log($"Invalid Drop. IsInShop: {!IsInShop}, draggedItem: {draggedItem?.Spell?.Title}");
        }
    }

    public static SpellUIItem GetDraggedItem()
    {
        return _draggedItem;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Spell == null)
            return;

        _tooltip.ShowTooltip(Spell, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Spell == null)
            return;

        _tooltip.HideTooltip();
    }
}
