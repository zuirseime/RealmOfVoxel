using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _camera;
    [SerializeField] private Entity _entity;

    private void Start()
    {
        _camera = Camera.main.transform;
        _entity.Died += OnEntityDied;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.forward);
    }

    private void OnEntityDied(object sender, EventArgs args)
    {
        GetComponent<Canvas>().enabled = false;
        enabled = false;
    }
}
