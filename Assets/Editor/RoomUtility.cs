using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class RoomUtility : Editor
{
    public static GameObject GetRoom<T>(string[] connectsTo) where T : Room
    {
        GameObject room = new GameObject(Regex.Replace(typeof(T).Name, "Room$", ""));
        room.AddComponent<T>().connectsTo = connectsTo.ToList();

        CreateDoors(room);
        CreateGeometry<T>(room);
        CreateBounds(room);
        CreateEnemyContainer(room);

        room.transform.localScale = Vector3.one * 0.25f;

        return room;
    }

    private static void CreateDoors(GameObject room)
    {
        GameObject doors = new GameObject("Doors");
        doors.transform.parent = room.transform;

        GameObject door = new GameObject("Door (1)");
        door.AddComponent<Door>();

        door.transform.parent = doors.transform;
        door.transform.localPosition = Vector3.forward * 8f;

        IconManager.SetIcon(door, IconManager.IconColor.Red, IconManager.IconType.Circle);

        GameObject cubePlaceholder = ObjectFactory.CreatePrimitive(PrimitiveType.Cube);
        cubePlaceholder.GetComponent<Renderer>().material = GetMaterialByName(nameof(Door));

        cubePlaceholder.transform.parent = door.transform;
        cubePlaceholder.transform.localPosition = new Vector3(0, 1.5f, -3.5f);
        cubePlaceholder.transform.localScale = new Vector3(4, 3, 3);
    }

    private static void CreateGeometry<T>(GameObject room)
    {
        GameObject geometry = new GameObject("Geometry");
        geometry.transform.parent = room.transform;

        GameObject box = ObjectFactory.CreatePrimitive(PrimitiveType.Cube);
        box.transform.parent = geometry.transform;
        box.transform.localScale = new Vector3(8, 6, 8);
        box.GetComponent<Renderer>().material = GetMaterialByName(Regex.Replace(typeof(T).Name, "Room$", ""));
    }

    private static void CreateBounds(GameObject room)
    {
        GameObject bounds = new GameObject("Bounds");
        bounds.transform.parent = room.transform;

        CreateBounds(bounds.transform, RoomBounds.BoundsType.Outer, IconManager.IconColor.Yellow, new Vector3(16, 12, 16));
        CreateBounds(bounds.transform, RoomBounds.BoundsType.Inner, IconManager.IconColor.Green, new Vector3(12, 6, 12));
    }

    private static void CreateBounds(Transform parent, RoomBounds.BoundsType type, IconManager.IconColor iconColor, Vector3 size)
    {
        GameObject bounds = new GameObject($"{type} Bounds");
        bounds.transform.parent = parent;
        //bounds.transform.localScale = size;

        bounds.AddComponent<BoxCollider>().size = size;
        bounds.AddComponent<RoomBounds>().type = type;

        IconManager.SetIcon(bounds, iconColor, IconManager.IconType.Diamond);
    }

    private static void CreateEnemyContainer(GameObject room)
    {
        GameObject container = new GameObject("Enemy Container");
        container.transform.parent = room.transform;

        container.AddComponent<EnemyContainer>();
    }

    private static Material GetMaterialByName(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Material>($"Assets/Resources/Materials/{name}.mat");
    }
}