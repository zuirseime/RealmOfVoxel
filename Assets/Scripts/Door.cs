using UnityEngine;

public class Door : MonoBehaviour
{
    [field: SerializeField] public bool Available { get; set; } = true;
    [field: SerializeField] public Door ConnectedDoor { get; set; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Connect(Door doorToConnect)
    {
        ConnectedDoor = doorToConnect;
        Available = false;
    }

    public bool Connects(Door doorToConnect)
    {
        RaycastHit hit;

        Vector3 offset = doorToConnect.transform.position - transform.position;
        Vector3 rayDirection = (transform.position - doorToConnect.transform.position).normalized;

        if (Physics.Raycast(transform.position, rayDirection, out hit))
        {
            SectionBounds bounds;
            if ((bounds = hit.collider.transform.parent.parent.GetComponentInChildren<SectionBounds>()) != null)
            {
                var thisParent = transform.parent.parent.GetComponent<Section>();
                var aotherParent = doorToConnect.transform.parent.parent.GetComponent<Section>();

                if (thisParent.bounds.Contains(bounds) || aotherParent.bounds.Contains(bounds))
                {
                    return false;
                }
            }
        }

        if (Vector3.Dot(transform.forward, doorToConnect.transform.forward) >= 0.9f)
        return false;

        if (Mathf.Approximately(offset.x, 0) || Mathf.Approximately(offset.z, 0))
        {
            return true;
        }
        
        Vector3 turnPoint1 = new Vector3(transform.position.x, 0, doorToConnect.transform.position.z);
        Vector3 turnPoint2 = new Vector3(doorToConnect.transform.position.x, 0, transform.position.z);

        bool path1 = CanMoveStraight(transform.position, turnPoint1) && CanMoveStraight(turnPoint1, doorToConnect.transform.position);
        bool path2 = CanMoveStraight(transform.position, turnPoint2) && CanMoveStraight(turnPoint2, doorToConnect.transform.position);

        return path1 || path2;
    }

    private bool CanMoveStraight(Vector3 start, Vector3 end)
    {
        return Mathf.Approximately(start.x, end.x) || Mathf.Approximately(start.z, end.z);
    }
}
