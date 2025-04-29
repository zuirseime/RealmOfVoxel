using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _camera;

    private void Start()
    {
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
}
