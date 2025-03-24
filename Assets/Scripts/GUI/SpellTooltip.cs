using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpellTooltip : MonoBehaviour
{
    [SerializeField] private SpellTooltipStatBox _statBoxPrefab;

    [SerializeField] private TextMeshProUGUI _spellName;
    [SerializeField] private TextMeshProUGUI _descritption;
    [SerializeField] private Transform _statsBox;

    [SerializeField] private Vector3 _offset;

    private List<SpellTooltipStatBox> _stats = new();

    private void Awake()
    {
        HideTooltip();
    }

    public void ShowTooltip(Spell spell, Vector3 position)
    {
        foreach (var stat in spell.Stats)
        {
            var statGO = Instantiate(_statBoxPrefab, _statsBox);
            statGO.Key.text = stat.Key;
            statGO.Value.text = stat.Value;
            _stats.Add(statGO);
        }

        _spellName.text = spell.SpellName;
        _descritption.text = spell.Description;

        gameObject.SetActive(true);
        transform.position = position + _offset;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        foreach (var stat in _stats)
        {
            Destroy(stat.gameObject);
        }
        _stats.Clear();
    }
}