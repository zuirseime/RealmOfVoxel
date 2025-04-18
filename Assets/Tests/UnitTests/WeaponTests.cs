using NUnit.Framework;
using UnityEngine;

public class WeaponTests
{
    private GameObject playerGO;
    private Player player;
    private EntityModifiers playerModifiers;
    private GameObject weaponGO;
    private Sword testSword;
    private GameObject targetGO;
    private Entity targetEntity;
    private EntityAttribute targetHealth;

    [SetUp]
    public void SetUp()
    {
        playerGO = new GameObject("Player");
        player = playerGO.AddComponent<Player>();
        playerModifiers = playerGO.GetComponent<EntityModifiers>();

        playerModifiers.DamageModifier = new EntityModifier();
        playerModifiers.CooldownModifier = new EntityModifier();
        playerModifiers.CritChanceModifier = new EntityModifier();
        playerModifiers.CritMultiplicationModifier = new EntityModifier();
        playerModifiers.MoveSpeedModifier = new EntityModifier();
        playerModifiers.DefenceModifier = new EntityModifier();
        playerModifiers.CoinModifier = new EntityModifier();
        playerModifiers.ResetModifiers();

        weaponGO = new GameObject("TestSword");
        testSword = weaponGO.AddComponent<Sword>();
        testSword.Owner = player;
        weaponGO.transform.SetParent(playerGO.transform);

        SetProtectedFieldValue(testSword, "_baseDamage", 10f);
        SetProtectedFieldValue(testSword, "_range", 3f);
        SetProtectedFieldValue(testSword, "_cooldown", 1f);
        SetProtectedFieldValue(testSword, "_critChance", 0.1f);
        SetProtectedFieldValue(testSword, "_critMultiplier", 2f);
        SetProtectedFieldValue(testSword, "_nextAttackTime", 0f);

        targetGO = new GameObject("Target");
        targetEntity = targetGO.AddComponent<DumyEnemy>();
        targetEntity.enabled = true;
        targetHealth = new EntityAttribute
        {
            MaxValue = 100f,
            Value = 100f
        };
        var targetModifiers = targetGO.GetComponent<EntityModifiers>();
        targetModifiers.DamageModifier = new EntityModifier();
        targetModifiers.CooldownModifier = new EntityModifier();
        targetModifiers.CritChanceModifier = new EntityModifier();
        targetModifiers.CritMultiplicationModifier = new EntityModifier();
        targetModifiers.MoveSpeedModifier = new EntityModifier();
        targetModifiers.DefenceModifier = new EntityModifier();
        targetModifiers.CoinModifier = new EntityModifier();
        targetModifiers.ResetModifiers();

        var healthField = typeof(DumyEnemy).GetField("_health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (healthField != null)
            healthField.SetValue(targetEntity, targetHealth);
        else
            Assert.Inconclusive("Cannot set _health field");
        
        var modifiersField = typeof(DumyEnemy).GetField("GetComponent<EntityModifiers>()", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance); // This won't work, GetComponent is a method
        
        targetGO.AddComponent<EntityModifiers>().DefenceModifier = new EntityModifier();
        targetGO?.GetComponent<EntityModifiers>()?.DefenceModifier?.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerGO);
        Object.DestroyImmediate(weaponGO);
        Object.DestroyImmediate(targetGO);
    }

    private void SetProtectedFieldValue<T>(T obj, string fieldName, object value)
    {
        var field = typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        } else
        {
            field = typeof(T).BaseType?.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            } else
            {
                Assert.Inconclusive($"Could not find field '{fieldName}' in type {typeof(T)} or its base type for testing.");
            }
        }
    }

    private object GetPrivateFieldValue<T>(T obj, string fieldName) where T : class
    {
        var field = typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null)
        {
            field = typeof(T).BaseType?.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }
        return field?.GetValue(obj);
    }

    [Test] // UT3_01
    public void Attack_WhenOnCooldown_DoesNotDamageTarget()
    {
        SetProtectedFieldValue(testSword, "_nextAttackTime", Time.time + 5f);
        float initialHealth = targetHealth.Value;

        testSword.Attack(targetEntity);

        Assert.AreEqual(initialHealth, targetHealth.Value, "Target health should not change when weapon is on cooldown.");
    }

    [Test]
    public void Attack_WhenCritOccurs_DealsCritDamageAndTriggersEvent()
    {
        SetProtectedFieldValue(testSword, "_critChance", 1.0f);
        SetProtectedFieldValue(testSword, "_nextAttackTime", 0f);
        float baseDamage = (float)GetPrivateFieldValue(testSword, "_baseDamage");
        float critMultiplier = (float)GetPrivateFieldValue(testSword, "_critMultiplier");
        float expectedDamage = baseDamage * critMultiplier;
        float initialHealth = targetHealth.Value;
        bool critEventTriggered = false;
        testSword.CritStrike += (sender, args) =>
        {
            critEventTriggered = true;
            Assert.AreEqual(critMultiplier, args.Multiplier, 0.01f, "Event multiplier mismatch.");
        };

        testSword.Attack(targetEntity);

        Assert.AreEqual(initialHealth - expectedDamage, targetHealth.Value, 0.01f, "Target health did not decrease by crit damage.");
        Assert.IsTrue(critEventTriggered, "CritStrike event should have triggered.");
        Assert.Greater((float)GetPrivateFieldValue(testSword, "_nextAttackTime"), 0f, "_nextAttackTime should be updated.");
    }

    [Test] // UT3_03
    public void Attack_WhenNoCritOccurs_DealsBaseDamageAndDoesNotTriggerEvent()
    {
        SetProtectedFieldValue(testSword, "_critChance", 0.0f);
        SetProtectedFieldValue(testSword, "_nextAttackTime", 0f);
        float baseDamage = (float)GetPrivateFieldValue(testSword, "_baseDamage");
        float expectedDamage = baseDamage;
        float initialHealth = targetHealth.Value;
        bool critEventTriggered = false;
        testSword.CritStrike += (sender, args) => critEventTriggered = true;

        testSword.Attack(targetEntity);

        Assert.AreEqual(initialHealth - expectedDamage, targetHealth.Value, 0.01f, "Target health did not decrease by base damage.");
        Assert.IsFalse(critEventTriggered, "CritStrike event should not have triggered.");
        Assert.Greater((float)GetPrivateFieldValue(testSword, "_nextAttackTime"), 0f, "_nextAttackTime should be updated.");
    }

    [Test] // UT3_04
    public void CanAttack_WithValidTarget_ReturnsTrue()
    {
        Assert.IsTrue(testSword.CanAttack(targetEntity));
    }

    [Test] // UT3_05
    public void CanAttack_WithNullTarget_ReturnsFalse()
    {
        Assert.IsFalse(testSword.CanAttack(null));
    }
}
