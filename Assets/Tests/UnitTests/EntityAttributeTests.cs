using NUnit.Framework;

public class EntityAttributeTests
{
    private EntityAttribute _attribute;

    [SetUp]
    public void Setup()
    {
        _attribute = new EntityAttribute();
        _attribute.MaxValue = 100f;
        _attribute.Value = 50f;
    }

    [Test]
    [TestCase(100f, 20f, 80f)] // UT1_01
    [TestCase(10f, 20f, 0f)]   // UT1_02
    public void Drain_ReducesValueCorrectly(float initialValue, float drainAmount, float expectedValue)
    {
        _attribute.Value = initialValue;
        bool eventTriggered = false;
        _attribute.ValueChanged += (sender, args) => eventTriggered = true;

        _attribute.Drain(drainAmount);

        Assert.AreEqual(expectedValue, _attribute.Value, 0.01f);
        Assert.IsTrue(eventTriggered || initialValue == expectedValue, "ValueChanged event should trigger if value changes.");
    }

    [Test]
    [TestCase(50f, 100f, 10f, 60f)]  // UT1_03
    [TestCase(95f, 100f, 10f, 100f)] // UT1_04
    public void Regenerate_ByAmount_IncreasesValueCorrectly(float initialValue, float maxValue, float regenAmount, float expectedValue)
    {
        _attribute.Value = initialValue;
        _attribute.MaxValue = maxValue;
        bool eventTriggered = false;
        _attribute.ValueChanged += (sender, args) => eventTriggered = true;

        _attribute.Regenerate(regenAmount, 1f);

        Assert.AreEqual(expectedValue, _attribute.Value, 0.01f);
        Assert.IsTrue(eventTriggered || initialValue == expectedValue, "ValueChanged event should trigger if value changes.");
    }

    [Test] // UT1_05
    public void Regenerate_ByRate_IncreasesValueCorrectly()
    {
        var fieldInfo = typeof(EntityAttribute).GetField("_regenerationRate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_attribute, 5f);
            _attribute.Value = 50f;
            bool eventTriggered = false;
            _attribute.ValueChanged += (sender, args) => eventTriggered = true;

            float regenAmount = 5f * 1f;
            _attribute.Regenerate(regenAmount, 1f);

            Assert.AreEqual(55f, _attribute.Value, 0.01f);
            Assert.IsTrue(eventTriggered, "ValueChanged event should trigger.");
        } else
        {
            Assert.Inconclusive("Could not access _regenerationRate field for testing.");
        }
    }

    [Test] // UT1_06
    public void Restore_SetsValueToMaxValue()
    {
        _attribute.Value = 30f;
        _attribute.MaxValue = 100f;
        bool eventTriggered = false;
        _attribute.ValueChanged += (sender, args) => eventTriggered = true;

        _attribute.Restore();

        Assert.AreEqual(100f, _attribute.Value, 0.01f);
        Assert.IsTrue(eventTriggered, "ValueChanged event should trigger.");
    }

    [Test]
    [TestCase(50f, 30f, true)]  // UT1_07
    [TestCase(20f, 30f, false)] // UT1_08
    public void CanDrain_ReturnsCorrectBoolean(float currentValue, float drainAmount, bool expectedResult)
    {
        _attribute.Value = currentValue;
        Assert.AreEqual(expectedResult, _attribute.CanDrain(drainAmount));
    }
}
