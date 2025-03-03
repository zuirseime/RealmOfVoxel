using UnityEngine;

public class CameraFollowState : CameraState
{
    private Transform _player;
    private float _followSpeed;

    public Vector3 offset;

    public CameraFollowState(CameraController controller, Transform player, float followSpeed) 
        : base(controller)
    {
        _player = player;
        _followSpeed = followSpeed;
    }

    public override void Update()
    {
        if (_player == null)
            return;

        Vector3 targetPosition = _player.position + offset;
        _controller.transform.position = Vector3.Lerp(_controller.transform.position, targetPosition, _followSpeed * Time.deltaTime);

        _controller.transform.LookAt(_player.position);
    }
}
