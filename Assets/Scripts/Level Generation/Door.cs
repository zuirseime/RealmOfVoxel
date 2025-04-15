using UnityEngine;

public class Door : MonoBehaviour
{
    public Room Parent { get; set; }

    [field: SerializeField] public bool Available { get; set; } = true;
    [field: SerializeField] public Door ConnectedDoor { get; set; }
    [field: SerializeField] public bool IsInterior { get; set; } = false;

    private Animator _animator;

    private void Awake()
    {
        Parent = transform.parent.parent.GetComponent<Room>();
        _animator = GetComponent<Animator>();
    }

    public void Close()
    {
        _animator.Play("DoorClose", 0, 0f);
    }

    public void Open()
    {
        _animator.Play("DoorOpen", 0, 0f);
    }
    
    public float GetAnimationState()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void Connect(Door doorToConnect)
    {
        ConnectedDoor = doorToConnect;
    }

    public bool Connects(Door doorToConnect)
    {
        if (Vector3.Dot(transform.forward, doorToConnect.transform.forward) >= 0.9f)
            return false;

        return true;
    }

    public bool ConnectsDirectly(Door doorToConnect)
    {
        Vector3 rayDirection = (transform.position - doorToConnect.transform.position).normalized;

        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit))
        {
            RoomBounds bounds;
            if ((bounds = hit.collider.transform.parent.parent.GetComponentInChildren<RoomBounds>()) != null)
            {
                var thisParent = transform.parent.parent.GetComponent<Room>();
                var aotherParent = doorToConnect.transform.parent.parent.GetComponent<Room>();

                if (!thisParent.bounds.Contains(bounds) && !aotherParent.bounds.Contains(bounds))
                {
                    if (bounds.type == RoomBounds.BoundsType.Inner)
                    {
                        return false;
                    }
                }
            }
        }

        return Connects(doorToConnect);
    }
}
