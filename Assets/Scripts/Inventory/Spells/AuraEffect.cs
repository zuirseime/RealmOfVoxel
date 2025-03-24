using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AuraEffect : SpellEffect
{
    protected virtual void Start()
    {
        Destroy();
    }

    protected virtual void Update()
    {
        
    }
}
