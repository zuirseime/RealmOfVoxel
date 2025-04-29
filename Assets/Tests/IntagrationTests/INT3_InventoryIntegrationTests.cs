using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.TestTools;

public class INT3_InventoryIntegrationTests
{
    private Player player;
    private Inventory inventory;
    private Wallet wallet;
    private WeaponEquipper weaponEquipper;
    private CharmEquipper charmEquipper;
    private GameObject surfaceGO, gameGO, playerGO, testWeaponGO, testCharmGO, testChestGO, testShopSlotGO, testEnemyGO;
    private Bow testWeapon;
    private LifeRing testCharm;
    private Chest testChest;
    private ShopSlot testShopSlot;
    private DummyEnemy testEnemy;
    private Game game;

    [SetUp]
    public void SetUp()
    {
        surfaceGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
        surfaceGO.transform.localScale = new Vector3(100, 1, 100);
        var surface = surfaceGO.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        TestHelper.LoadResource(out game, "Game");
        TestHelper.InvokePrivateMethod(game, "Awake");

        TestHelper.LoadResource(out player, "Entities/Player");
        TestHelper.SetEntityModifiers(player);

        inventory = player.GetComponent<Inventory>();
        weaponEquipper = player.GetComponent<WeaponEquipper>();
        wallet = player.GetComponent<Wallet>();
        charmEquipper = player.GetComponent<CharmEquipper>();

        TestHelper.InvokePrivateMethod(inventory, "Awake");
        TestHelper.InvokePrivateMethod(weaponEquipper, "Start");

        inventory.CurrentWeapon.Owner = player;

        TestHelper.LoadResource(out testWeapon, "Weapons/Bow");
        TestHelper.SetProtectedFieldValue(testWeapon, "_name", "Test Sword");
        TestHelper.SetProtectedFieldValue(testWeapon, "_price", 50);
        TestHelper.InvokePrivateMethod(testWeapon, "Awake");
        testWeapon.Owner = player;
        TestHelper.InvokePrivateMethod(testWeapon, "Start");

        TestHelper.LoadResource(out testCharm, "Charms/LifeRing");
        TestHelper.SetProtectedFieldValue(testCharm, "_name", "Test Ring");
        TestHelper.SetProtectedFieldValue(testCharm, "_price", 30);
        TestHelper.InvokePrivateMethod(testCharm, "Awake");
        TestHelper.InvokePrivateMethod(testCharm, "Start");

        TestHelper.LoadResource(out testChest, "Decorations/Chest");
        testChest.StoredWeapon = testWeapon;
        testChest.StoredCoins = 25;
        TestHelper.InvokePrivateMethod(testChest, "Start");

        TestHelper.LoadResource(out testShopSlot, "Decorations/Shop Slot");
        testShopSlot.SetCollectable(testWeapon);

        TestHelper.LoadResource(out testEnemy, "Entities/Enemies/Enemy");
        testEnemy.Health.Value = 10;
        TestHelper.SetProtectedFieldValue(testEnemy, "_minCoins", 10);
        TestHelper.SetProtectedFieldValue(testEnemy, "_maxCoins", 10);
        TestHelper.InvokePrivateMethod(testEnemy, "Awake");

        player.transform.position = Vector3.zero;
        testWeapon.transform.position = Vector3.right * 2;
        testCharm.transform.position = Vector3.left * 2;
        testChest.transform.position = Vector3.forward * 2;
        testShopSlot.transform.position = Vector3.back * 2;
        testEnemy.transform.position = Vector3.one * 5;

        SetWalletCoins(wallet, 100f);
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
    }

    private IEnumerator MovePlayerTo(Vector3 targetPosition)
    {
        player.transform.position = targetPosition;
        yield return null;
    }

    [UnityTest]
    public IEnumerator INT3_01_PickupWeapon_EquipsWeaponAndDestroysItem()
    {
        Weapon initialWeapon = weaponEquipper.CurrentWeapon;
        Assert.IsTrue(testWeapon.gameObject.activeSelf);

        yield return MovePlayerTo(testWeapon.transform.position);
        Collectable collectable = testWeapon.GetComponent<Collectable>();
        Assert.IsNotNull(collectable);
        TestHelper.SetProtectedFieldValue(collectable, "_playerWithinReach", player);
        collectable.Collect(player);
        yield return null;

        Assert.IsNotNull(weaponEquipper.SecondaryWeapon?.Title);
        Assert.IsTrue((bool)TestHelper.GetPrivateFieldValue(collectable, "_taken"));
    }

    [UnityTest]
    public IEnumerator INT3_02_PickupCharm_EquipsCharmAndAppliesEffect()
    {
        Charm initialCharm = charmEquipper.Charm;
        float initialMaxHealth = player.Health.MaxValue;
        Assert.IsTrue(testCharm.gameObject.activeSelf);

        yield return MovePlayerTo(testCharm.transform.position);
        Collectable collectable = testCharm.GetComponent<Collectable>();
        TestHelper.SetProtectedFieldValue(collectable, "_playerWithinReach", player);
        collectable.Collect(player);
        yield return null;

        Assert.IsNotNull(charmEquipper.Charm, "Charm should be equipped.");
        Assert.AreEqual(testCharm.Title, charmEquipper.Charm.Title);
        Assert.AreNotSame(testCharm, charmEquipper.Charm);
        Assert.Greater(player.Health.MaxValue, initialMaxHealth, "Max health should increase after equipping LifeRing.");
        Assert.IsTrue((bool)TestHelper.GetPrivateFieldValue(collectable, "_taken"));
    }

    [UnityTest]
    public IEnumerator INT3_03_KillEnemy_AddsCoinsToWallet()
    {
        float initialCoins = wallet.Coins;
        int enemyCoins = testEnemy.Coins;
        Assert.Greater(enemyCoins, 0);

        player.GetComponent<EntityModifiers>().DamageModifier.Value = 100f;
        player.Target = testEnemy;

        player.Attack();
        yield return null;

        float expectedCoins = initialCoins + enemyCoins * player.GetComponent<EntityModifiers>().CoinModifier.Value;
        testEnemy.Died += (sender, args) =>
        {
            Assert.AreEqual(expectedCoins, wallet.Coins, 0.01f, "Coins should increase after killing enemy.");
            player.GetComponent<EntityModifiers>().ResetModifiers();
        };
    }
}
