using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _spellImage;
    [SerializeField] private Slider _cooldown;
    [SerializeField] private TextMeshProUGUI _keybind;
    [SerializeField] private Image _border;

    private SpellTooltip _tooltip;
    private Spell _spell;

    public Spell Spell => _spell;

    public void Ininialize(Spell spell, string keybind, SpellTooltip tooltip)
    {
        _tooltip = tooltip;
        _spell = spell;
        _keybind.text = keybind;
        _cooldown.value = 0;

        if (_spell != null)
        {
            _spellImage.sprite = spell.Sprite;
            _spellImage.color = Color.white;
            _spell.SpellUsed += OnSpellUsed;
            _spell.SpellSelected += OnSpellSelected;
            _spell.SpellDeselected += OnSpellDeselected;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_spell == null)
            return;

        _tooltip.ShowTooltip(_spell, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_spell == null)
            return;

        _tooltip.HideTooltip();
    }

    private void OnSpellSelected(object sender, SpellEventArgs args)
    {
        _border.gameObject.SetActive(true);
    }

    private void OnSpellDeselected(object sender, SpellEventArgs args)
    {
        _border.gameObject.SetActive(false);
    }

    private void OnSpellUsed(object sender, SpellEventArgs args)
    {
        OnSpellDeselected(_spell, args);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        float cooldownTime = _spell.Cooldown;
        float timer = cooldownTime;
        
        while (timer > 0)
        {
            _cooldown.value = timer / cooldownTime;
            timer -= Time.deltaTime;
            yield return null;
        }

        _cooldown.value = 0;
    }
}
