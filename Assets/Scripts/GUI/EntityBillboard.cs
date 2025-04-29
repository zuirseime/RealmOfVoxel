using System;
using UnityEngine;

public class EntityBillboard : MonoBehaviour
{
    private Transform _camera;
    [SerializeField] private Entity _entity;

    private void Start()
    {
        _entity.Died += OnEntityDied;
        if (Camera.main == null)
        {
            Debug.Log("No main camera found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
            return;
        }
        _camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_camera != null)
            transform.LookAt(transform.position + _camera.forward);
    }

    private void OnEntityDied(object sender, EventArgs args)
    {
        GetComponent<Canvas>().enabled = false;
        enabled = false;
    }
}
