using UnityEngine;

public class SpellInventory : MonoBehaviour
{
    [SerializeField] private SpellSlot[] _spellSlots;
    [SerializeField] private SpellTooltip _tooltip;

    private void Start()
    {
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        var spells = Game.Instance.CurrentSpellSet;
        var userInput = Settings.Instance.Input;

        string[] keybinds = new string[]
        {
            userInput.Spell1.ToString(),
            userInput.Spell2.ToString(),
            userInput.Spell3.ToString(),
            userInput.Spell4.ToString()
        };

        for (int i = 0; i < _spellSlots.Length; i++)
        {
            _spellSlots[i].Ininialize(spells[i], keybinds[i], _tooltip);
        }
    }
}
