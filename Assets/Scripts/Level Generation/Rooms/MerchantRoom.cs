using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MerchantRoom : Room
{
    [SerializeField] private ShopSlot[] _goods;

    public override void Prepare()
    {
        base.Prepare();

        Queue<Collectable> collectables = new(FindObjectOfType<Game>().GetCollectables(_goods.Length).ToArray());
        foreach (ShopSlot good in _goods)
        {
            good.SetCollectable(collectables.Dequeue());
        }
    }
}
