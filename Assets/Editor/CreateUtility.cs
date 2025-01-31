using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CreateUtility : Editor
{
    [MenuItem("GameObject/Tools/Room")]
    public static void CreateRoom(MenuCommand menuCommand)
    {
        GameObject room = new("Room");

        CreateDoors(room);
        CreateGeometry(room);
        CreateBounds(room);

        Place(room);
    }

    public static void Place(GameObject gObj)
    {
        gObj.transform.position = Vector3.zero;
        StageUtility.PlaceGameObjectInCurrentStage(gObj);
        GameObjectUtility.EnsureUniqueNameForSibling(gObj);

        Undo.RegisterCreatedObjectUndo(gObj, $"Create Object: {gObj.name}");
        Selection.activeGameObject = gObj;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private static void CreateDoors(GameObject room)
    {
        GameObject doors = new("Doors");
        doors.transform.parent = room.transform;

        GameObject door = new("Door (1)");
        door.transform.position = Vector3.forward * 0.5f;
        door.transform.parent = doors.transform;
        IconManager.SetIcon(door, IconManager.IconColor.Red, IconManager.IconType.Circle);
    }

    private static void CreateGeometry(GameObject room)
    {
        GameObject geometry = new("Geometry");
        geometry.transform.parent = room.transform;

        GameObject box = ObjectFactory.CreatePrimitive(PrimitiveType.Cube);
        box.transform.parent = geometry.transform;
    }

    private static void CreateBounds(GameObject room)
    {
        GameObject bounds = new("Bounds");
        bounds.transform.parent = room.transform;
        bounds.AddComponent<BoxCollider>();
        IconManager.SetIcon(bounds, IconManager.IconColor.Green, IconManager.IconType.Diamond);
    }
}
