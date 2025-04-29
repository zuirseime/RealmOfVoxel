using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEngine;

public class INT2_SpellSystemIntegrationTests
{
    private Player player;
    private GameObject playerGO, surfaceGO, gameGO;
    private Spell shieldSpellSO;
    private Spell[] activeSpells;

    [SetUp]
    public void SetUp()
    {
        surfaceGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
        surfaceGO.transform.localScale = new Vector3(100, 1, 100);
        var surface = surfaceGO.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        gameGO = new GameObject("GameGO");
        gameGO.AddComponent<SpellManager>();
        var game = gameGO.AddComponent<Game>();
        TestHelper.InvokePrivateMethod(game, "Awake");

        playerGO = new GameObject("PlayerGO");
        player = playerGO.AddComponent<Player>();
        TestHelper.SetEntityModifiers(player);
        TestHelper.InvokePrivateMethod(player.GetComponent<Inventory>(), "Awake");
        TestHelper.InvokePrivateMethod(player, "Start");
        TestHelper.InvokePrivateMethod(player.GetComponent<WeaponEquipper>(), "Start");

        player.enabled = true;
        var attribute = new EntityAttribute
        {
            MaxValue = 1000000f,
            Value = 1000000f,
        };

        TestHelper.SetProtectedFieldValue(player, "_health", attribute);
        TestHelper.SetProtectedFieldValue(player, "_mana", attribute);
        TestHelper.SetProtectedFieldValue(player, "_mana._regenerationRate", 0);

        shieldSpellSO = ScriptableObject.CreateInstance<ShieldSpell>();
        SetSpellProperties(shieldSpellSO, "divine-shield", 15f, 5.0f, 0f);
        TestHelper.SetProtectedFieldValue(shieldSpellSO, "_nextCastTime", 0f);

        activeSpells = new Spell[4];
        activeSpells[0] = shieldSpellSO;
        player.GetComponent<SpellCaster>().SetSpells(activeSpells);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(playerGO);
        Object.Destroy(surfaceGO);
        Object.Destroy(gameGO);
        ScriptableObject.DestroyImmediate(shieldSpellSO);
    }

    private void SetSpellProperties(Spell spell, string id, float manaCost, float cooldown, float range)
    {
        spell.ID = id;
        TestHelper.SetProtectedFieldValue(spell, "_manaCost", manaCost);
        TestHelper.SetProtectedFieldValue(spell, "_cooldown", cooldown);
        TestHelper.SetProtectedFieldValue(spell, "_range", range);
        TestHelper.SetProtectedFieldValue(spell, "_nextCastTime", 0f);
    }

    [Test]
    public void INT2_01_UseSpell_ChangesStateAndCastsAndConsumesMana()
    {
        float initialMana = player.Mana.Value;
        Assert.Greater(initialMana, shieldSpellSO.ManaCost);

        shieldSpellSO.SpellUsed += (sender, args) =>
        {
            Assert.Less(player.Mana.Value, initialMana, "Mana should decrease after casting.");
            Assert.AreEqual(initialMana - shieldSpellSO.ManaCost, player.Mana.Value, 0.01f);

            PlayerState state = TestHelper.GetPrivateFieldValue(player, "_currentState") as PlayerState;
            Assert.IsInstanceOf<PlayerCastingState>(state, "Player should change to casting state.");

            float nextCastTime = (float)TestHelper.GetPrivateFieldValue(shieldSpellSO, "_nextCastTime");
            Assert.Greater(nextCastTime, Time.time, "Cooldown should be set for the future.");

            Assert.IsNotNull(GameObject.FindObjectOfType<ShieldEffect>(), "ShieldEffect should be instantiated.");
        };

        player.GetComponent<SpellCaster>().UseSpell(0);
    }

    [Test]
    public void INT2_02_UseSpell_OnCooldown_DoesNotCast()
    {
        float nextCastTimeAfterFirst = 0;
        float manaAfterFirstCast = 0;
        shieldSpellSO.SpellUsed += (sender, args) =>
        {
            manaAfterFirstCast = player.Mana.Value;
            nextCastTimeAfterFirst = (float)TestHelper.GetPrivateFieldValue(shieldSpellSO, "_nextCastTime");
            Assert.Greater(nextCastTimeAfterFirst, Time.time, "Cooldown should be set for the future.");
        };

        player.GetComponent<SpellCaster>().UseSpell(0);

        shieldSpellSO.SpellUsed -= null;

        shieldSpellSO.SpellUsed += (sender, args) =>
        {
            Assert.AreEqual(manaAfterFirstCast, player.Mana.Value, "Mana should not change when casting on cooldown.");
            float nextCastTimeAfterSecond = (float)TestHelper.GetPrivateFieldValue(shieldSpellSO, "_nextCastTime");
            Assert.AreEqual(nextCastTimeAfterFirst, nextCastTimeAfterSecond, "Cooldown time should not change.");
        };

        player.GetComponent<SpellCaster>().UseSpell(0);
    }
}
