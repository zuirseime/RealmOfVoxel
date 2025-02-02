using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CreateUtility : Editor
{
    [MenuItem("GameObject/Tools/Section")]
    public static void CreateSection(MenuCommand menuCommand)
    {
        GameObject section = new("Section");
        section.AddComponent<Section>();

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
        door.tag = "Door";
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
        box.GetComponent<Renderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Materials/Spawn.mat");
    }

    private static void CreateBounds(GameObject room)
    {
        GameObject bounds = new("Bounds");
        bounds.transform.parent = room.transform;
        bounds.AddComponent<BoxCollider>();
        bounds.AddComponent<SectionBounds>();
        IconManager.SetIcon(bounds, IconManager.IconColor.Green, IconManager.IconType.Diamond);
    }
}
