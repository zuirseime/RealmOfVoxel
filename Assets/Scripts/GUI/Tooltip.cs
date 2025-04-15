using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TooltipStatBox _statBoxPrefab;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Transform _costContainer;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _descritption;
    [SerializeField] private Transform _statsBox;

    [SerializeField] private Vector3 _offset;

    private List<TooltipStatBox> _stats = new();

    private void Awake()
    {
        HideTooltip();
    }

    public void ShowTooltip(IDisplayable displayable, Vector3 position)
    {
        foreach (var stat in displayable.Stats)
        {
            var statGO = Instantiate(_statBoxPrefab, _statsBox);
            statGO.Key.text = stat.Key;
            statGO.Value.text = stat.Value;
            _stats.Add(statGO);
        }

        _title.text = displayable.Title;
        _descritption.text = displayable.Description;
        
        if (displayable is Spell spell)
        {
            _costText.text = spell.Price.ToString();
            bool acquired = Game.Instance.SpellManager.GetSpellData(spell.ID).Acquired;
            _costContainer.gameObject.SetActive(!acquired);
        }

        gameObject.SetActive(true);
        transform.position = Vector3Int.RoundToInt(position + _offset);
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