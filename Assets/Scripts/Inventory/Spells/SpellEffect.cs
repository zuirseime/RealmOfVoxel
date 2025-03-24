using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class SpellEffect : MonoBehaviour
{
    public Entity Owner { get; set; }
    public float Duration { get; set; }
    public float Radius { get; set; }

    protected virtual void Destroy()
    {
        Destroy(gameObject, Duration + 0.5f);
    }
}