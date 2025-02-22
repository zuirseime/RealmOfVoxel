using UnityEngine;

public class Door : MonoBehaviour
{
    [field: SerializeField] public bool Available { get; set; } = true;
    [field: SerializeField] public Door ConnectedDoor { get; set; }

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
            SectionBounds bounds;
            if ((bounds = hit.collider.transform.parent.parent.GetComponentInChildren<SectionBounds>()) != null)
            {
                var thisParent = transform.parent.parent.GetComponent<Room>();
                var aotherParent = doorToConnect.transform.parent.parent.GetComponent<Room>();

                if (!thisParent.bounds.Contains(bounds) && !aotherParent.bounds.Contains(bounds))
                {
                    if (bounds.type == SectionBounds.BoundsType.Inner)
                    {
                        return false;
                    }
                }
            }
        }

        return Connects(doorToConnect);
    }
}
