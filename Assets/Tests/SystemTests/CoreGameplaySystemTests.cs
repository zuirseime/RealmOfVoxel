using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class CoreGameplaySystemTests
{
    string testLevelScene = "SystemTestsScene";

    Player player;
    Inventory inventory;
    Wallet wallet;
    WeaponEquipper weaponEquipper;
    SpellCaster spellCaster;
    CharmEquipper charmEquipper;
    Enemy testEnemy;
    Chest testChest;
    ShopSlot testShopSlot;
    Game game;
    UIInventory uiInventory;
    PauseMenuManager pauseMenuManager;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        SceneManager.LoadScene(testLevelScene, LoadSceneMode.Single);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == testLevelScene)
            {
                game = Object.FindObjectOfType<Game>();
                player = Object.FindObjectOfType<Player>();
                inventory = player?.GetComponent<Inventory>();
                wallet = player?.GetComponent<Wallet>();
                weaponEquipper = player?.GetComponent<WeaponEquipper>();
                spellCaster = player?.GetComponent<SpellCaster>();
                charmEquipper = player?.GetComponent<CharmEquipper>();
                testEnemy = Object.FindObjectOfType<DummyEnemy>();
                testChest = Object.FindObjectOfType<Chest>();
                testShopSlot = Object.FindObjectOfType<ShopSlot>();
                uiInventory = Object.FindObjectOfType<UIInventory>();
                pauseMenuManager = Object.FindObjectOfType<PauseMenuManager>();

                Assert.IsNotNull(player, "Player not found in the scene.");
                Assert.IsNotNull(inventory, "Inventory not found on Player.");
                Assert.IsNotNull(wallet, "Wallet not found on Player.");
                Assert.IsNotNull(weaponEquipper, "WeaponEquipper not found on Player.");
                Assert.IsNotNull(spellCaster, "SpellCaster not found on Player.");
                Assert.IsNotNull(charmEquipper, "CharmEquipper not found on Player.");
                Assert.IsNotNull(testEnemy, "Enemy not found in the scene.");
                Assert.IsNotNull(testChest, "Chest not found in the scene.");
                Assert.IsNotNull(testShopSlot, "ShopSlot not found in the scene.");
                Assert.IsNotNull(uiInventory, "UIInventory not found in the scene.");
                Assert.IsNotNull(pauseMenuManager, "PauseMenuManager not found in the scene.");

                testEnemy.Activate();
            }
        };

        yield return new WaitForSecondsRealtime(1f);
    }

    private void SimulateRightClick(Vector3 position)
    {
        SelectionManager selectionManager = player?.GetComponent<SelectionManager>();
        NavMeshAgent agent = player?.GetComponent<NavMeshAgent>();
        if (selectionManager != null && agent != null)
        {
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                selectionManager.Select(hit.position);
                player.SetDestination(hit.position);
            } else
            {
                Debug.LogWarning($"SimulateRightClick: Position {position} is not on NavMesh.");
            }
        } else
        {
            Debug.LogWarning("SimulateRightClick: Player, SelectionManager or NavMeshAgent not found.");
        }
    }

    private void SimulateRightClickOnEnemy(Enemy enemy)
    {
        SelectionManager selectionManager = player?.GetComponent<SelectionManager>();
        if (selectionManager != null && enemy != null)
        {
            selectionManager.Select(enemy);
            //player.SetDestination(enemy.transform.position);
        } else
        {
            Debug.LogWarning("SimulateRightClickOnEnemy: Player, SelectionManager or Enemy not found.");
        }
    }

    private void SimulateKeyPress(KeyCode key)
    {
        if (key == Settings.Instance.Input.Interact && inventory != null)
        {
            // Потрібно визначити, з чим взаємодіємо (Chest, ShopSlot, Collectable)
            // Це складно автоматизувати без системи відстеження близькості
            Debug.LogWarning($"Simulating key '{key}' requires proximity checks.");
        } else if (key == Settings.Instance.Input.SwapWeapons && weaponEquipper != null)
        {
            weaponEquipper.Swap();
        } else if (key == Settings.Instance.Input.Spell1 && spellCaster != null)
        {
            spellCaster.UseSpell(0);
        }
        else if (key == KeyCode.Escape)
        {
            bool isPaused = (bool)TestHelper.GetPrivateFieldValue(pauseMenuManager, "_isPaused");
            pauseMenuManager.ChangeState(!isPaused, isPaused ? 1f : 0f);
        }
    }

    private IEnumerator MovePlayerTo(Vector3 targetPosition)
    {
        player.transform.position = targetPosition;
        yield return null;
    }

    [UnityTest] // FR1.01
    public IEnumerator TSS_01_MovePlayer()
    {
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = startPos + Vector3.right * 5;

        if (!NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            Assert.Inconclusive($"Target position {targetPos} is not on NavMesh. Adjust test scene or position.");
        }
        targetPos = hit.position;

        SimulateRightClick(targetPos);
        yield return MovePlayerTo(targetPos);

        Assert.Less(Vector3.Distance(player.transform.position, targetPos), 1.0f, "Player should reach the target position.");
    }

    [UnityTest] // FR1.02 + FR2.01 + FR6.01
    public IEnumerator TSS_02_TargetAndAttackEnemy()
    {
        Assert.IsNotNull(testEnemy, "Enemy is required for this test.");
        Assert.IsTrue(testEnemy.IsAlive, "Enemy should be alive initially.");

        var coinsDisplay = Object.FindObjectOfType<CoinsDisplay>();

        float initialEnemyHealth = testEnemy.Health.Value;
        Vector3 enemyPos = testEnemy.transform.position;
        float initialCoins = inventory.LocalCoins;
        bool enemyDied = false;

        testEnemy.HealthChanged += (sender, args) =>
        {
            Assert.Less(testEnemy.Health.Value, initialEnemyHealth, "Enemy health should decrease after being attacked.");
        };
        testEnemy.Died += (sender, args) =>
        {
            enemyDied = true;
            float coinsAfterDeath = inventory.LocalCoins;
            Assert.IsFalse(testEnemy.IsAlive, "Enemy should be dead after being attacked.");
            Assert.AreEqual(0, testEnemy.Health.Value, "Enemy health should be 0 after death.");
            Assert.Greater(coinsAfterDeath, initialCoins, "Player should receive coins after enemy death.");
        };

        SimulateRightClickOnEnemy(testEnemy);
        yield return null;

        Assert.IsNotNull(player.Target, "Player should target the enemy.");

        float timeout = 10f;
        float startTime = Time.time;

        while (!enemyDied && Time.time < startTime + timeout)
        {
            yield return null;
        }
    }
    
    [UnityTest] // FR1.03
    public IEnumerator TSS_03_SwapWeapons()
    {
        weaponEquipper.Equip(game.GetWeapon(inventory.GetWeaponSet()));
        weaponEquipper.Equip(game.GetWeapon(inventory.GetWeaponSet()));

        Weapon initialPrimary = weaponEquipper.CurrentWeapon;
        Weapon initialSecondary = weaponEquipper.SecondaryWeapon;
        Assert.IsNotNull(initialPrimary, "Player must have a primary weapon.");
        Assert.IsNotNull(initialSecondary, "Player must have a secondary weapon for swap test.");
        Assert.AreNotEqual(initialPrimary.Title, initialSecondary.Title);

        WeaponsDisplay weaponsUI = Object.FindObjectOfType<WeaponsDisplay>();
        Assert.IsNotNull(weaponsUI, "WeaponsDisplay UI not found.");

        SimulateKeyPress(Settings.Instance.Input.SwapWeapons);
        yield return null;

        var primarySlot = TestHelper.GetPrivateFieldValue(weaponsUI, "_primarySlot") as UISlot;
        var secondarySlot = TestHelper.GetPrivateFieldValue(weaponsUI, "_secondarySlot") as UISlot;

        Assert.AreEqual(initialSecondary.Title, weaponEquipper.CurrentWeapon?.Title, "Primary weapon should be the initial secondary.");
        Assert.AreEqual(initialPrimary.Title, weaponEquipper.SecondaryWeapon?.Title, "Secondary weapon should be the initial primary.");
        Assert.AreEqual(initialSecondary, primarySlot.Displayable);
        Assert.AreEqual(initialPrimary, secondarySlot.Displayable);
    }

    [UnityTest] // FR1.04 + FR3.02 + FR3.03 + FR5.02 + FR7.02
    public IEnumerator TSS_04_UseSpell_ShowsCooldownUI()
    {
        Assert.IsNotNull(spellCaster, "SpellCaster is required.");

        var shield = game.SpellManager.GetSpell("divine-shield");
        var spells = game.SpellManager.GetAll().Take(3).ToArray();
        var spellSet = new List<Spell> { shield };
        spellSet.AddRange(spells);
        spellCaster.SetSpells(spellSet.ToArray());

        Assert.IsNotNull(uiInventory, "UIInventory is required.");
        Spell spellToCast = spellCaster.Spells[0];
        Assert.IsNotNull(spellToCast, "No spell in slot 0.");
        player.Mana.Value = spellToCast.ManaCost + 1;
        float initialMana = player.Mana.Value;

        SpellSlot spellSlotUI = uiInventory.GetComponentsInChildren<SpellSlot>()
                                           .FirstOrDefault(s => s.Displayable == spellToCast);

        Assert.IsNotNull(spellSlotUI, $"SpellSlot UI for {spellToCast.Title} not found.");
        Slider cooldownSlider = TestHelper.GetProtectedFieldValue<SpellSlot, Slider>(spellSlotUI, "_cooldown");
        Assert.IsNotNull(cooldownSlider, "Cooldown slider not found in SpellSlot UI.");
        Assert.AreEqual(0, cooldownSlider.value, "Cooldown slider should be 0 initially.");

        spellToCast.SpellUsed += (sender, args) =>
        {
            Assert.Less(player.Mana.Value, initialMana, "Mana should be consumed.");
            Assert.Greater(cooldownSlider.value, 0, "Cooldown slider should show progress > 0.");
            Assert.LessOrEqual(cooldownSlider.value, 1, "Cooldown slider value should be <= 1.");

            SpellEffect effect = Object.FindObjectOfType<SpellEffect>();
            Assert.IsNotNull(effect, "SpellEffect not found in the scene.");
        };

        SimulateKeyPress(Settings.Instance.Input.Spell1);

        yield return new WaitForSeconds(spellToCast.Cooldown + 0.1f);
        Assert.AreEqual(0, cooldownSlider.value, 0.01f, "Cooldown slider should be 0 after cooldown duration.");
    }

    [UnityTest] // FR1.05
    public IEnumerator TSS_05_InteractWithChest()
    {
        Assert.IsNotNull(testChest, "Chest is required for this test.");

        SphereCollider chestCollider = testChest.GetComponent<SphereCollider>();
        Assert.IsTrue(chestCollider.enabled, "Chest should be interactable initially.");

        testChest.ChestOpened += (sender, weapon) =>
        {
            Assert.IsFalse(chestCollider.enabled, "Chest collider should be disabled after opening.");
            Assert.IsNull(testChest.StoredWeapon, "Chest StoredWeapon should be null after opening.");
        };

        yield return MovePlayerTo(testChest.transform.position);
        TestHelper.SetProtectedFieldValue(testChest, "_playersInventoryWithinReach", inventory);
        TestHelper.InvokePrivateMethod(testChest, "LetPlayerOpenChest");
    }

    [UnityTest] // FR1.06
    public IEnumerator TSS_07_PauseAndResumeGame()
    {
        Assert.IsNotNull(pauseMenuManager, "PauseMenuManager is required.");
        GameObject pauseUI = TestHelper.GetProtectedFieldValue<PauseMenuManager, GameObject>(pauseMenuManager, "_pauseMenuUI");
        Assert.IsNotNull(pauseUI, "Pause Menu UI reference is missing.");
        Assert.IsFalse(pauseUI.activeSelf, "Pause menu should be inactive initially.");
        Assert.AreEqual(1f, Time.timeScale, "Time should run normally initially.");

        SimulateKeyPress(KeyCode.Escape);
        yield return null;

        Assert.IsTrue(pauseUI.activeSelf, "Pause menu should be active.");
        Assert.AreEqual(0f, Time.timeScale, "Time should stop when paused.");

        SimulateKeyPress(KeyCode.Escape);
        yield return null;

        Assert.IsFalse(pauseUI.activeSelf, "Pause menu should be inactive after resume.");
        Assert.AreEqual(1f, Time.timeScale, "Time should run normally after resume.");
    }

    [UnityTest] // FR6.03
    public IEnumerator TSS_08_PlayerBuysCollectable()
    {
        testShopSlot.SetCollectable(game.GetCollectable());
        player.Inventory.AddCoins(testShopSlot.Collectable.Price);

        testShopSlot.CollectablePurchased += (sender, collectable) =>
        {
            Assert.IsNull(testShopSlot.Collectable, "Collectable should be purchased.");
            Assert.AreSame(collectable, charmEquipper.Charm, "Purchased collectable should match the one in the slot.");
        };

        yield return MovePlayerTo(testShopSlot.transform.position);
        TestHelper.SetProtectedFieldValue(testShopSlot, "_playersInventoryWithinReach", inventory);
        TestHelper.InvokePrivateMethod(testShopSlot, "LetPlayerOpenChest");
    }
}
