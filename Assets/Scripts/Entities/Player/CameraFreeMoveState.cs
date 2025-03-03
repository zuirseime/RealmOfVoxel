using UnityEngine;

public class CameraFreeMoveState : CameraState
{
    private Transform _player;
    private Vector3 _lastMousePosition;
    private bool _isDragging;
    private float _dragSpeed;
    private float _maxDistance;

    public CameraFreeMoveState(CameraController controller, Transform player, float dragSpeed, float maxDistance) : base(controller)
    {
        _player = player;
        _dragSpeed = dragSpeed;
        _maxDistance = maxDistance;
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _isDragging = true;
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            _isDragging = false;
        }

        if (_isDragging)
        {
            Vector3 delta = Input.mousePosition - _lastMousePosition;
            _controller.transform.position -= new Vector3(delta.x * _dragSpeed, 0, delta.y * _dragSpeed);
            _lastMousePosition = Input.mousePosition;

            LimitDistance();
        }
    }

    private void LimitDistance()
    {
        Vector3 playerPos = new Vector3(_player.position.x, 0, _player.position.z);
        Vector3 cameraPos = new Vector3(_controller.transform.position.x, 0, _controller.transform.position.z);

        if (Vector3.Distance(cameraPos, playerPos) > _maxDistance)
        {
            Vector3 direction = (cameraPos - playerPos).normalized;
            var newCameraPos = playerPos + direction * _maxDistance;

            newCameraPos.y = _controller.transform.position.y;
            _controller.transform.position = newCameraPos;
        }
    }
}