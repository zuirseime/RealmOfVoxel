using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CreateUtility : Editor
{
    [MenuItem("GameObject/Tools/Section")]
    public static void CreateSection(MenuCommand menuCommand)
    {
        GameObject section = new("Section");
        section.AddComponent<Room>();

        CreateDoors(section);
        CreateGeometry(section);
        CreateBounds(section);

        Place(section);
    }

    [MenuItem("GameObject/Tools/Level")]
    public static void CreateLevel(MenuCommand menuCommand)
    {
        GameObject level = new("Level");
        level.AddComponent<LevelGenerator>();

        Place(level);
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
        door.AddComponent<Door>();
        door.transform.position = Vector3.forward * 0.5f;
        door.transform.parent = doors.transform;
        IconManager.SetIcon(door, IconManager.IconColor.Red, IconManager.IconType.Circle);

        GameObject cube = ObjectFactory.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = door.transform;
        cube.GetComponent<Renderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Materials/Door.mat");
    }

    private static void CreateGeometry(GameObject room)
    {
        GameObject geometry = new("Geometry");
        geometry.transform.parent = room.transform;

        GameObject box = ObjectFactory.CreatePrimitive(PrimitiveType.Cube);
        box.transform.parent = geometry.transform;
        box.GetComponent<Renderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Materials/Spawn.mat");
    }

    private static void CreateBounds(GameObject room)
    {
        GameObject bounds = new("Bounds");
        bounds.transform.parent = room.transform;

        CreateBounds(bounds.transform, SectionBounds.BoundsType.Outer, IconManager.IconColor.Green);
        CreateBounds(bounds.transform, SectionBounds.BoundsType.Inner, IconManager.IconColor.Yellow);
    }

    private static void CreateBounds(Transform parent, SectionBounds.BoundsType type, IconManager.IconColor iconColor)
    {
        GameObject bounds = new($"{type} Bounds");
        bounds.transform.parent = parent;

        bounds.AddComponent<BoxCollider>();
        bounds.AddComponent<SectionBounds>().type = type;
        IconManager.SetIcon(bounds, iconColor, IconManager.IconType.Diamond);
    }
}
