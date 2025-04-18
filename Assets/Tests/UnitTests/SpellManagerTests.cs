using NUnit.Framework;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "TestSpell", menuName = "Spells/TestSpell")]
public class TestSpell : Spell
{
    protected override void ApplyEffect(Entity owner, Vector3 targetPosition) { }
}

public class SpellManagerTests
{
    private GameObject managerGO;
    private SpellManager spellManager;
    private TestSpell fireballSpell;
    private TestSpell shieldSpell;
    private TestSpell devourSpell;

    [SetUp]
    public void SetUp()
    {
        fireballSpell = ScriptableObject.CreateInstance<TestSpell>();
        fireballSpell.ID = "fireball";
        fireballSpell.name = "Fireball";

        shieldSpell = ScriptableObject.CreateInstance<TestSpell>();
        shieldSpell.ID = "divine-shield";
        shieldSpell.name = "Shield";

        devourSpell = ScriptableObject.CreateInstance<TestSpell>();
        devourSpell.ID = "devour";
        devourSpell.name = "Devour";

        managerGO = new GameObject("SpellManagerGO");
        spellManager = managerGO.AddComponent<SpellManager>();

        var allSpellsField = typeof(SpellManager).GetField("_allSpells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (allSpellsField != null)
        {
            allSpellsField.SetValue(spellManager, new Spell[] { fireballSpell, shieldSpell, devourSpell });
        } else
        { Assert.Inconclusive("Cannot set _allSpells field."); }

        
        PlayerPrefs.DeleteKey("PlayerSpellData");

        var awakeMethod = typeof(SpellManager).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awakeMethod?.Invoke(spellManager, null);
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteKey("PlayerSpellData");
        ScriptableObject.DestroyImmediate(fireballSpell);
        ScriptableObject.DestroyImmediate(shieldSpell);
        ScriptableObject.DestroyImmediate(devourSpell);
        UnityEngine.Object.DestroyImmediate(managerGO);
    }

    private Dictionary<string, SpellData> GetSpellDataDict(SpellManager manager)
    {
        var dictField = typeof(SpellManager).GetField("_spellDataDict", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return dictField?.GetValue(manager) as Dictionary<string, SpellData>;
    }

    [Test] // UT4_01
    public void SetSpellAcquired_SetsFlagAndSaves()
    {
        spellManager.SetSpellAcquired("fireball", true);
        var dict = GetSpellDataDict(spellManager);

        Assert.IsTrue(dict["fireball"].Acquired);

        string json = PlayerPrefs.GetString("PlayerSpellData");
        Assert.IsNotNull(json);
        var loadedData = JsonConvert.DeserializeObject<List<SpellData>>(json);
        Assert.IsTrue(loadedData.First(s => s.SpellId == "fireball").Acquired);
    }

    [Test] // UT4_02
    public void SetSpellActive_SetsFlagSlotAndSaves()
    {
        spellManager.SetSpellAcquired("fireball", true);
        spellManager.SetSpellActive("fireball", true, 0);
        var dict = GetSpellDataDict(spellManager);

        Assert.IsTrue(dict["fireball"].Active);
        Assert.AreEqual(0, dict["fireball"].Slot);

        string json = PlayerPrefs.GetString("PlayerSpellData");
        Assert.IsNotNull(json);
        var loadedData = JsonConvert.DeserializeObject<List<SpellData>>(json);
        var fireballData = loadedData.First(s => s.SpellId == "fireball");
        Assert.IsTrue(fireballData.Active);
        Assert.AreEqual(0, fireballData.Slot);
    }

    [Test] // UT4_03
    public void SetSpellActive_False_ResetsFlagSlotAndSaves()
    {
        spellManager.SetSpellAcquired("fireball", true);
        spellManager.SetSpellActive("fireball", true, 0);

        spellManager.SetSpellActive("fireball", false);
        var dict = GetSpellDataDict(spellManager);

        Assert.IsFalse(dict["fireball"].Active);
        Assert.AreEqual(-1, dict["fireball"].Slot);

        string json = PlayerPrefs.GetString("PlayerSpellData");
        Assert.IsNotNull(json);
        var loadedData = JsonConvert.DeserializeObject<List<SpellData>>(json);
        var fireballData = loadedData.First(s => s.SpellId == "fireball");
        Assert.IsFalse(fireballData.Active);
        Assert.AreEqual(-1, fireballData.Slot);
    }


    [Test] // UT4_04
    public void GetAcquired_ReturnsOnlyAcquiredSpells()
    {
        spellManager.SetSpellAcquired("fireball", true);
        spellManager.SetSpellAcquired("divine-shield", false);

        Spell[] acquiredSpells = spellManager.GetAcquired();

        Assert.AreEqual(1, acquiredSpells.Length);
        Assert.AreEqual("fireball", acquiredSpells[0].ID);
    }

    [Test] // UT4_05
    public void GetActive_ReturnsCorrectSpellsInCorrectSlots()
    {
        spellManager.SetSpellAcquired("fireball", true);
        spellManager.SetSpellActive("fireball", true, 0);
        spellManager.SetSpellAcquired("divine-shield", true);
        spellManager.SetSpellActive("divine-shield", false);
        spellManager.SetSpellAcquired("devour", true);
        spellManager.SetSpellActive("devour", true, 2);

        Spell[] activeSpells = spellManager.GetActive();

        Assert.AreEqual(4, activeSpells.Length);
        Assert.IsNotNull(activeSpells[0]);
        Assert.AreEqual("fireball", activeSpells[0].ID);
        Assert.IsNull(activeSpells[1]);
        Assert.IsNotNull(activeSpells[2]);
        Assert.AreEqual("devour", activeSpells[2].ID);
        Assert.IsNull(activeSpells[3]);
    }

    [Test] // UT4_06 & UT4_07 combined
    public void SaveAndLoadSpellData_RestoresState()
    {
        spellManager.SetSpellAcquired("fireball", true);
        spellManager.SetSpellActive("fireball", true, 1);
        spellManager.SetSpellAcquired("divine-shield", true);
        spellManager.SetSpellActive("divine-shield", false);
        spellManager.SetSpellAcquired("devour", false);

        GameObject newManagerGO = new GameObject("NewSpellManager");
        SpellManager newSpellManager = newManagerGO.AddComponent<SpellManager>();

        var allSpellsFieldNew = typeof(SpellManager).GetField("_allSpells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (allSpellsFieldNew != null)
            allSpellsFieldNew.SetValue(newSpellManager, new Spell[] { fireballSpell, shieldSpell, devourSpell });
        
        var awakeMethodNew = typeof(SpellManager).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awakeMethodNew?.Invoke(newSpellManager, null);

        var newDict = GetSpellDataDict(newSpellManager);
        Assert.IsTrue(newDict["fireball"].Acquired);
        Assert.IsTrue(newDict["fireball"].Active);
        Assert.AreEqual(1, newDict["fireball"].Slot);

        Assert.IsTrue(newDict["divine-shield"].Acquired);
        Assert.IsFalse(newDict["divine-shield"].Active);
        Assert.AreEqual(-1, newDict["divine-shield"].Slot);

        Assert.IsFalse(newDict["devour"].Acquired);
        Assert.IsFalse(newDict["devour"].Active);
        Assert.AreEqual(-1, newDict["devour"].Slot);

        UnityEngine.Object.DestroyImmediate(newManagerGO);
    }

    [Test] // UT4_08
    public void ResetAll_ClearsAcquiredAndActiveFlagsAndSaves()
    {
        spellManager.SetSpellAcquired("fireball", true);
        spellManager.SetSpellActive("fireball", true, 0);
        spellManager.SetSpellAcquired("divine-shield", true);

        spellManager.ResetAll();

        var dict = GetSpellDataDict(spellManager);
        Assert.IsFalse(dict["fireball"].Acquired);
        Assert.IsFalse(dict["fireball"].Active);
        Assert.IsFalse(dict["divine-shield"].Acquired);
        Assert.IsFalse(dict["divine-shield"].Active);

        string json = PlayerPrefs.GetString("PlayerSpellData");
        Assert.IsNotNull(json);
        var loadedData = JsonConvert.DeserializeObject<List<SpellData>>(json);
        Assert.IsFalse(loadedData.Any(s => s.Acquired || s.Active));
    }
}