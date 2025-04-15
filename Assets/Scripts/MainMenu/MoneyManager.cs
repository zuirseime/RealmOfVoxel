using System;
using UnityEngine;

public static class MoneyManager
{
    private const string MoneyKey = "PlayerMoney";
    private const float CoinsToMoneyConversionFactor = 0.25f;

    private static int _currentMoney = 0;

    public static int CurrentMoney
    {
        get => _currentMoney;
        private set
        {
            if (_currentMoney != value)
            {
                _currentMoney = value;
                OnMoneyChanged?.Invoke(_currentMoney);
                SaveMoney();
            }
        }
    }

    public static event Action<int> OnMoneyChanged;

    public static void LoadMoney()
    {
        CurrentMoney = PlayerPrefs.GetInt(MoneyKey, 0);
    }

    public static void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, CurrentMoney);
    }

    public static void ConvertCoinsToMoney(int coins)
    {
        if (coins < 0)
            return;
        CurrentMoney += Mathf.RoundToInt(coins * CoinsToMoneyConversionFactor);
        Debug.Log($"Converted {coins} coins to money. Current money: {CurrentMoney}");
    }

    public static void AddMoney(int amount)
    {
        if (amount < 0)
            return;

        CurrentMoney += amount;
        Debug.Log($"Added {amount}. Current money: {CurrentMoney}");
    }

    public static bool SpendMoney(int amount)
    {
        if (amount < 0)
            return false;

        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            Debug.Log($"Spent {amount}. Current money: {CurrentMoney}");
            return true;
        }
        Debug.Log($"Not enough money. Have {CurrentMoney}, need {amount}");
        return false;
    }

    public static void DropMoney()
    {
        CurrentMoney = 0;
        SaveMoney();
    }
}
