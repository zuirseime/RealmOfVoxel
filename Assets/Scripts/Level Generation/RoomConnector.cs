using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomConnector
{
    [HideInInspector] public List<Room> rooms;

    public void Connect()
    {
        Debug.Log("Connecting rooms...");
        foreach (var giver in rooms.Where(g => g.Available()).OrderBy(g => g.spawnIndex))
        {
            var receivers = rooms.Where(r => r != giver).Where(r => r.Available())
                                     .Where(r => !giver.IsAlreadyConnected(r))
                                     .Where(r => r.Connects(giver) && giver.Connects(r)).ToArray();

            foreach (var receiver in receivers.OrderBy(r => Vector3.Distance(giver.transform.position, r.transform.position)))
            {
                if (receiver.doorsClosed)
                    continue;

                var giverDoor = giver.GetNearestDoor(receiver);
                var receiverDoor = receiver.GetNearestDoor(giver);

                if (giverDoor == null || receiverDoor == null)
                    continue;

                if (giverDoor.Connects(receiverDoor))
                {
                    giverDoor.Connect(receiverDoor);
                    receiverDoor.Connect(giverDoor);

                    if (Random.value < giver.chanceToCloseDoors)
                    {
                        giver.doorsClosed = true;
                        break;
                    }
                }
            }
        }

        SealUnusedDoors();

        if (rooms.Any(r => r.doors.Count == 0))
        {
            throw new System.Exception("Some rooms are locked!");
        }
    }

    private void SealUnusedDoors()
    {
        rooms.ForEach(r => r.SealUnusedDoors());
    }
}
