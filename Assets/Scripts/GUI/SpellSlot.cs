using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlot : UISlot
{
    [SerializeField] private Image _background;
    [SerializeField] private Slider _cooldown;
    [SerializeField] private Image _border;

    public override void Initialize(IDisplayable displayable, string keyBind = "")
    {
        base.Initialize(displayable, keyBind);

        _cooldown.value = 0;

        if (_displayable is Spell spell)
        {
            spell.SpellUsed += OnSpellUsed;
            spell.SpellSelected += OnSpellSelected;
            spell.SpellDeselected += OnSpellDeselected;
        }
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
        OnSpellDeselected(_displayable as Spell, args);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        float cooldownTime = (_displayable as Spell).Cooldown;
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
