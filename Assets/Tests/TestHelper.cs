using UnityEngine;
using UnityEngine.Assertions;

public static class TestHelper
{
    public static void LoadResource<T>(out T component, string path) where T : MonoBehaviour
    {
        component = Object.Instantiate(Resources.Load<T>(path));
    }

    public static void SetEntityModifiers<T>(T entity) where T : Entity
    {
        var modifiers = entity.GetComponent<EntityModifiers>();

        modifiers.DamageModifier = new EntityModifier();
        modifiers.CooldownModifier = new EntityModifier();
        modifiers.CritChanceModifier = new EntityModifier();
        modifiers.CritMultiplicationModifier = new EntityModifier();
        modifiers.MoveSpeedModifier = new EntityModifier();
        modifiers.DefenceModifier = new EntityModifier();
        modifiers.CoinModifier = new EntityModifier();
        modifiers.ResetModifiers();
    }

    public static void InvokePrivateMethod<T>(T obj, string methodName, params object[] parameters)
    {
        var method = typeof(T).GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(obj, parameters);
    }

    public static void SetProtectedFieldValue<T>(T obj, string fieldName, object value) where T : class
    {
        var field = typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (field == null && typeof(T).BaseType != null)
        {
            field = typeof(T).BaseType.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        }
        if (field != null)
            field.SetValue(obj, value);
        else
            Debug.LogWarning($"Field '{fieldName}' not found in {typeof(T)} or base types.");
    }

    public static object GetPrivateFieldValue<T>(T obj, string fieldName) where T : class
    {
        var field = typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null)
        {
            field = typeof(T).BaseType?.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }
        return field?.GetValue(obj);
    }

    public static TField GetProtectedFieldValue<TClass, TField>(TClass obj, string fieldName) where TClass : class
    {
        var field = typeof(TClass).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null && typeof(TClass).BaseType != null)
        {
            field = typeof(TClass).BaseType.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }
        if (field != null)
        {
            return (TField)field.GetValue(obj);
        }
        return default(TField);
    }
}
