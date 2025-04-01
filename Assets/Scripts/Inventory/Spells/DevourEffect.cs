using System.Collections;
using UnityEngine;

public class DevourEffect : AreaEffect
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _effectHeight;

    public float Damage { get; set; }

    protected override void Start()
    {
        base.Start();

        _particleSystem.Stop();
        var main = _particleSystem.main;
        main.duration = Duration;
        _particleSystem.Play();

        StartCoroutine(ExpansionRoutine());
    }

    protected override void AffectEntity(Entity entity)
    {
        if (entity != null && entity.GetType() != Owner.GetType())
            entity.TakeDamage(Owner, Damage);
    }

    private IEnumerator ExpansionRoutine()
    {
        float startSize = 1;
        float timer = 0;

        float scaleDuration = Duration / 4;

        while (timer < scaleDuration)
        {
            float scaleFactor = Mathf.Lerp(startSize, MaxExpansion, timer / scaleDuration);
            transform.localScale = Vector3.one * scaleFactor;

            var shape = _particleSystem.shape;
            shape.radius = scaleFactor;

            _particleSystem.transform.position = new Vector3(
                _particleSystem.transform.position.x, 
                _effectHeight, 
                _particleSystem.transform.position.z
            );

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
