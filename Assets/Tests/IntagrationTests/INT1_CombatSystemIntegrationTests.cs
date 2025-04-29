using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.AI.Navigation;

public class INT1_CombatSystemIntegrationTests
{
    private Player player;
    private Enemy enemy;
    private Sword sword;
    private Bow bow;
    private MightnessAmulet mightnessAmulet;
    private LivingMagma livingMagma;
    private GameObject playerGO, enemyGO, swordGO, bowGO, mightnessAmuletGO, livingMagmaGO, surfaceGO;

    [SetUp]
    public void SetUp()
    {
        surfaceGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
        surfaceGO.transform.localScale = new Vector3(100, 1, 100);
        var surface = surfaceGO.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        var gameGO = new GameObject("GameGO");
        gameGO.AddComponent<SpellManager>();
        var game = gameGO.AddComponent<Game>();
        TestHelper.InvokePrivateMethod(game, "Awake");

        playerGO = new GameObject("PlayerGO");
        playerGO.AddComponent<BoxCollider>();
        player = playerGO.AddComponent<Player>();
        TestHelper.SetEntityModifiers(player);
        TestHelper.InvokePrivateMethod(player, "Start");
        var inventory = playerGO.GetComponent<Inventory>();
        TestHelper.InvokePrivateMethod(inventory, "Awake");

        if (!player.TryGetComponent<WeaponEquipper>(out var weaponEquipper))
            weaponEquipper = player.gameObject.AddComponent<WeaponEquipper>();
        TestHelper.InvokePrivateMethod(weaponEquipper, "Start");

        var weaponContainerGO = new GameObject("WeaponContainerGO");
        TestHelper.SetProtectedFieldValue(weaponEquipper, "_container", weaponContainerGO.transform);

        enemyGO = new GameObject("EnemyGO");
        enemyGO.AddComponent<BoxCollider>();
        enemy = enemyGO.AddComponent<DummyEnemy>();
        TestHelper.SetEntityModifiers(enemy);
        TestHelper.InvokePrivateMethod(enemy, "Awake");
        TestHelper.SetProtectedFieldValue(enemy, "_attackDamage", 10f);

        swordGO = new GameObject("SwordGO");
        sword = swordGO.AddComponent<Sword>();
        sword.Owner = player;
        TestHelper.SetProtectedFieldValue(sword, "_baseDamage", 1f);

        bowGO = new GameObject("BowGO");
        bow = bowGO.AddComponent<Bow>();
        bow.Owner = player;
        TestHelper.SetProtectedFieldValue(bow, "_baseDamage", 1f);
        TestHelper.SetProtectedFieldValue(bow, "_arrowPrefab", Resources.Load<Projectile>("Weapons/Arrow"));

        mightnessAmuletGO = new GameObject("MightnessAmuletGO");
        mightnessAmulet = mightnessAmuletGO.AddComponent<MightnessAmulet>();
        TestHelper.SetProtectedFieldValue(mightnessAmulet, "_bonus", 100);

        livingMagmaGO = new GameObject("LivingMagmaGO");
        livingMagma = livingMagmaGO.AddComponent<LivingMagma>();
        TestHelper.SetProtectedFieldValue(livingMagma, "_bonus", 20);

        playerGO.transform.position = Vector3.zero;
        enemyGO.transform.position = Vector3.forward * 2;

        player.enabled = true;
        enemy.enabled = true;
        var health = new EntityAttribute
        {
            MaxValue = 1000000f,
            Value = 1000000f
        };

        TestHelper.SetProtectedFieldValue(player, "_health", health);
        TestHelper.SetProtectedFieldValue(enemy, "_health", health);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(playerGO);
        Object.Destroy(enemyGO);
        Object.Destroy(swordGO);
        Object.Destroy(bowGO);
        Object.Destroy(mightnessAmuletGO);
        Object.Destroy(livingMagmaGO);
        Object.Destroy(surfaceGO);
    }

    [UnityTest]
    public IEnumerator INT1_01_PlayerAttacksEnemyWithSword_EnemyTakesDamage()
    {
        float initialEnemyHealth = enemy.Health.Value;
        player.GetComponent<WeaponEquipper>().Equip(sword);
        player.Target = enemy;

        player.Attack();
        yield return null;

        Assert.Less(enemy.Health.Value, initialEnemyHealth, "Enemy health should decrease.");
    }

    [UnityTest]
    public IEnumerator INT1_02_PlayerAttacksEnemyWithBow_ProjectileHitsAndDamages()
    {
        float initialEnemyHealth = enemy.Health.Value;
        player.GetComponent<WeaponEquipper>().Equip(bow);
        player.Target = enemy;
        enemyGO.transform.position = Vector3.forward * 5;

        player.Attack();

        yield return new WaitForSeconds(1.0f);

        Assert.Less(enemy.Health.Value, initialEnemyHealth, "Enemy health should decrease after projectile hit.");
    }

    [Test]
    public void INT1_03_CritDamage()
    {
        player.GetComponent<WeaponEquipper>().Equip(sword);
        var weapon = player.Inventory.CurrentWeapon;

        var modifiers = player.GetComponent<EntityModifiers>();
        modifiers.CritChanceModifier.Value = 10f;

        float initialPlayerDamage = weapon.Damage;
        weapon.CritStrike += (sender, args) =>
        {
            Assert.Greater(args.Damage, initialPlayerDamage, "Weapon damage should increase when critical strike.");
        };

        player.Target = enemy;
        enemyGO.transform.position = Vector3.forward * 1;

        player.Attack();
    }

    [UnityTest]
    public IEnumerator INT1_04_EnemyAttacksPlayer_PlayerTakesDamage()
    {
        float initialPlayerHealth = player.Health.Value;

        enemy.Target = player;
        enemyGO.transform.position = Vector3.forward * 1;

        enemy.Attack();
        yield return new WaitForSeconds(enemy.AttackCooldown + 0.1f);

        Assert.Less(player.Health.Value, initialPlayerHealth, "Player health should decrease.");
    }


    [UnityTest]
    public IEnumerator INT1_05_EnemyDies_WhenHealthReachesZero()
    {
        Assert.IsTrue(enemy.IsAlive);

        float massiveDamage = enemy.Health.MaxValue + 10;

        enemy.TakeDamage(player, massiveDamage);
        yield return null;

        Assert.AreEqual(0, enemy.Health.Value, "Enemy health should be 0.");
        Assert.IsFalse(enemy.IsAlive, "Enemy should not be alive.");
        Assert.IsFalse(enemy.enabled);
    }

    [UnityTest]
    public IEnumerator INT1_06_EquipMightnessAmulet_IncreasesDamage()
    {
        player.GetComponent<WeaponEquipper>().Equip(sword);
        player.Target = enemy;

        float healthBeforeCharm = enemy.Health.Value;
        player.Attack();
        yield return new WaitForSeconds(0.1f);
        float damageWithoutCharm = healthBeforeCharm - enemy.Health.Value;
        enemy.Health.Value = enemy.Health.MaxValue;
        yield return new WaitForSeconds(sword.Cooldown + 0.1f);

        player.GetComponent<CharmEquipper>().Equip(mightnessAmulet);
        yield return null;

        float healthBeforeCharmAttack = enemy.Health.Value;
        player.Attack();
        yield return new WaitForSeconds(0.1f);
        float damageWithCharm = healthBeforeCharmAttack - enemy.Health.Value;

        Assert.Greater(damageWithCharm, damageWithoutCharm, "Damage with MightnessAmulet should be higher.");
    }

    [UnityTest]
    public IEnumerator INT1_07_EquipLivingMagma_DecreaseTakenDamage()
    {
        enemy.Target = player;

        float healthBeforeCharm = player.Health.Value;
        enemy.Attack();
        yield return new WaitForSeconds(0.1f);
        float damageWithoutCharm = healthBeforeCharm - player.Health.Value;
        player.Health.Value = player.Health.MaxValue;

        player.GetComponent<CharmEquipper>().Equip(livingMagma);
        yield return null;

        float healthBeforeCharmAttack = player.Health.Value;
        enemy.Attack();
        yield return new WaitForSeconds(0.1f);
        float damageWithCharm = healthBeforeCharmAttack - player.Health.Value;

        Assert.Greater(damageWithoutCharm, damageWithCharm, "Damage with MightnessAmulet should be higher.");
    }
}
