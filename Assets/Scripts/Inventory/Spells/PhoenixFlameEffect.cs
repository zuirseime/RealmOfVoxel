using System.Collections;
using UnityEngine;

public class PhoenixFlameEffect : AreaEffect
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject _plane;

    public float Heal { get; set; }
    public float RotationSpeed { get; set; }

    protected override void Start()
    {
        base.Start();

        transform.localScale = Vector3.one * MaxExpansion;

        _particleSystem.Stop();
        var main = _particleSystem.main;
        main.duration = Duration;
        _particleSystem.Play();

        var shape = _particleSystem.shape;
        shape.radius = MaxExpansion;
    }

    protected override void Update()
    {
        _plane.transform.Rotate(RotationSpeed * Time.deltaTime * Vector3.up);
        base.Update();
    }

    protected override void AffectEntity(Entity entity)
    {
        if (entity != null && entity.GetType() == Owner.GetType())
            entity.Heal(Heal);
    }
}