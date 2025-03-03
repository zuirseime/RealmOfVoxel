using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;

    [Header("Follow Mode Settings")]
    [SerializeField] private Vector3 _followOffset;
    [SerializeField] private float _followSpeed;

    [Header("Free Move Settings")]
    [SerializeField] private float _dragSpeed;
    [SerializeField] private float _maxDistance;

    [Header("Zoom Settings")]
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _minHieght;
    [SerializeField] private float _maxHeight;

    private Camera _camera;
    private CameraState _currentState;
    private CameraFollowState _followState;
    private CameraFreeMoveState _freeMoveState;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _followState = new CameraFollowState(this, _player, _followSpeed);
        _freeMoveState = new CameraFreeMoveState(this, _player, _dragSpeed, _maxDistance);

        _currentState = _followState;
    }

    private void Update()
    {
        _followState.offset = _followOffset;

        _currentState?.Update();

        HandleZoom();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchState(_currentState is CameraFollowState ? _freeMoveState : _followState);
        }
    }

    private void SwitchState(CameraState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            _followOffset.y -= scroll * _zoomSpeed;
            _followOffset.y = Mathf.Clamp(_followOffset.y, _minHieght, _maxHeight);
        }
    }
}
