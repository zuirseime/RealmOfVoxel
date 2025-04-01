using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public event EventHandler<CoinsEventArgs> CoinsChanged;

    [SerializeField] private float _coins;

    public float Coins
    {
        get => _coins;
        private set
        {
            if (_coins != value)
            {
                var last = _coins;
                _coins = value;
                CoinsChanged?.Invoke(this, new CoinsEventArgs(last, _coins));
            }
        }
    }

    public bool CanSpend(int amount) => Coins >= amount;

    public void Add(float amount) => Coins += amount;

    public void Spend(int amount)
    {
        if (CanSpend(amount))
            Coins -= amount;
    }
}