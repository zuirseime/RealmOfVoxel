using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileEffect : SpellEffect
{
    protected Vector3 _targetPosition;
    protected Vector3 _startPosition;
    protected Rigidbody _rigidbody;

    public float MoveSpeed { get; set; }
    public float Damage { get; set; }

    protected virtual void Start()
    {
        _startPosition = new Vector3(transform.position.x, 1f, transform.position.z);
    }

    public void SetTarget(Vector3 targetPosition)
    {
        _rigidbody = GetComponent<Rigidbody>();

        _targetPosition = targetPosition;
        Vector3 direction = (_targetPosition - transform.position).normalized;
        _rigidbody.velocity = direction * MoveSpeed;
    }

    protected virtual void Update() { }
}
