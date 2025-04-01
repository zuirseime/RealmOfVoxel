using UnityEngine;

public class CharmSlot : UISlot
{
    private void Awake()
    {
        var playerInventory = FindObjectOfType<Inventory>();
        playerInventory.CharmChanged += OnCharmChanged;
    }

    private void OnCharmChanged(object sender, Charm charm)
    {
        
    }
}
