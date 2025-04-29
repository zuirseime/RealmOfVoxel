using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private SpellSlot[] _spellSlots;
    [SerializeField] private UISlot _charmSlot;

    private void OnEnable()
    {
        var playerInventory = FindObjectOfType<Inventory>();
        playerInventory.SpellsChanged += OnSpellSetChanged;
        playerInventory.CharmChanged += OnCharmChanged;
    }

    private void OnSpellSetChanged(object sender, SpellSetEventArgs args)
    {
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
            _spellSlots[i].Initialize(args.Spells[i], keybinds[i]);
        }
    }

    private void OnCharmChanged(object sender, Charm charm)
    {
        _charmSlot.Initialize(charm);
    }
}
