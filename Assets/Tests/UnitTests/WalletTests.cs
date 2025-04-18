using NUnit.Framework;
using UnityEngine;

public class WalletTests
{
    private Wallet _wallet;
    private float _lastCoinsValue;
    private bool _coinsChangedEventTriggered;

    [SetUp]
    public void Setup()
    {
        GameObject go = new GameObject();
        _wallet = go.AddComponent<Wallet>();

        _coinsChangedEventTriggered = false;
        _lastCoinsValue = -1f;
        _wallet.CoinsChanged += CoinsChanged;

        SetWalletCoins(_wallet, 0f);
    }

    [TearDown]
    public void TearDown()
    {
        if (_wallet != null && _wallet.gameObject != null)
        {
            UnityEngine.Object.DestroyImmediate(_wallet.gameObject);
        }
        _wallet.CoinsChanged -= CoinsChanged;
    }

    private void CoinsChanged(object sender, CoinsEventArgs e)
    {
        _coinsChangedEventTriggered = true;
        _lastCoinsValue = e.Last;
    }

    private void SetWalletCoins(Wallet instance, float value)
    {
        var fieldInfo = typeof(Wallet).GetField("_coins", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(instance, value);
        } else
        {
            Assert.Inconclusive("Could not access _coins field for testing.");
        }

        _coinsChangedEventTriggered = false;
    }

    [Test] // UT2_01
    public void Add_IncreasesCoinsAndTriggersEvent()
    {
        SetWalletCoins(_wallet, 100f);
        float initialCoins = _wallet.Coins;

        _wallet.Add(50f);

        Assert.AreEqual(initialCoins + 50f, _wallet.Coins, 0.01f);
        Assert.IsTrue(_coinsChangedEventTriggered, "CoinsChanged event should trigger.");
        Assert.AreEqual(initialCoins, _lastCoinsValue, 0.01f, "Event should report correct previous value.");
    }

    [Test] // UT2_02
    public void Spend_SufficientCoins_DecreasesCoinsAndTriggersEvent()
    {
        SetWalletCoins(_wallet, 100f);
        float initialCoins = _wallet.Coins;

        _wallet.Spend(30);

        Assert.AreEqual(initialCoins - 30f, _wallet.Coins, 0.01f);
        Assert.IsTrue(_coinsChangedEventTriggered, "CoinsChanged event should trigger.");
        Assert.AreEqual(initialCoins, _lastCoinsValue, 0.01f, "Event should report correct previous value.");
    }

    [Test] // UT2_03
    public void Spend_InsufficientCoins_DoesNotChangeCoinsAndNotTriggerEvent()
    {
        SetWalletCoins(_wallet, 20f);
        float initialCoins = _wallet.Coins;

        _wallet.Spend(30);

        Assert.AreEqual(initialCoins, _wallet.Coins, 0.01f);
        Assert.IsFalse(_coinsChangedEventTriggered, "CoinsChanged event should not trigger.");
    }

    [Test]
    [TestCase(50, 30, true)]  // UT2_04
    [TestCase(20, 30, false)] // UT2_05
    public void CanSpend_ReturnsCorrectBoolean(float currentCoins, int spendAmount, bool expectedResult)
    {
        SetWalletCoins(_wallet, currentCoins);
        Assert.AreEqual(expectedResult, _wallet.CanSpend(spendAmount));
    }

    [Test]
    public void Add_NegativeAmount_DoesNotChangeCoins()
    {
        SetWalletCoins(_wallet, 100f);
        float initialCoins = _wallet.Coins;

        _wallet.Add(-50f);

        Assert.AreEqual(initialCoins - 50f, _wallet.Coins, 0.01f);
        Assert.IsTrue(_coinsChangedEventTriggered, "CoinsChanged event should trigger if value changes.");
    }

    [Test]
    public void Spend_NegativeAmount_DoesNotChangeCoins()
    {
        SetWalletCoins(_wallet, 100f);
        float initialCoins = _wallet.Coins;

        _wallet.Spend(-30);

        Assert.AreEqual(initialCoins, _wallet.Coins, 0.01f);
        Assert.IsFalse(_coinsChangedEventTriggered);
    }
}
