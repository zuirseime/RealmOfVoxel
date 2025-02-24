using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CreateUtility : Editor
{
    [MenuItem("GameObject/Current Project/Spawn Room", false, 50)]
    public static void CreateSpawn(MenuCommand menuCommand)
    {
        GameObject room = RoomUtility.GetRoom<SpawnRoom>(new string[] { "SmallTrial", "MediumTrial" });

        Place(room);
    }

    [MenuItem("GameObject/Current Project/Boss Room", false, 51)]
    public static void CreateBoss(MenuCommand menuCommand)
    {
        GameObject room = RoomUtility.GetRoom<BossRoom>(new string[] { "Merchant", "Treasure", "SmallTrial", "MediumTrial", "BigTrial" });

        Place(room);
    }

    [MenuItem("GameObject/Current Project/Treasure Room", false, 52)]
    public static void CreateTreasure(MenuCommand menuCommand)
    {
        GameObject room = RoomUtility.GetRoom<TreasureRoom>(new string[] { "Boss", "Merchant", "SmallTrial", "MediumTrial", "BigTrial" });

        Place(room);
    }

    [MenuItem("GameObject/Current Project/Merchant Room", false, 53)]
    public static void CreateMerchant(MenuCommand menuCommand)
    {
        GameObject room = RoomUtility.GetRoom<MerchantRoom>(new string[] { "Boss", "Treasure", "SmallTrial", "MediumTrial", "BigTrial" });

        Place(room);
    }

    [MenuItem("GameObject/Current Project/Small Trial Room", false, 54)]
    public static void CreateSmallTrial(MenuCommand menuCommand)
    {
        GameObject room = RoomUtility.GetRoom<SmallTrialRoom>(new string[] { "Spawn", "Boss", "Merchant", "Treasure", "SmallTrial", "MediumTrial", "BigTrial" });

        Place(room);
    }

    [MenuItem("GameObject/Current Project/Medium Trial Room", false, 55)]
    public static void CreateMediumTrial(MenuCommand menuCommand)
    {
        GameObject room = RoomUtility.GetRoom<MediumTrialRoom>(new string[] { "Spawn", "Boss", "Merchant", "Treasure", "SmallTrial", "MediumTrial", "BigTrial" });

        Place(room);
    }

    [MenuItem("GameObject/Current Project/Big Trial Room", false, 56)]
    public static void CreateBigTrial(MenuCommand menuCommand)
    {
        GameObject room = RoomUtility.GetRoom<BigTrialRoom>(new string[] { "Spawn", "Boss", "Merchant", "Treasure", "SmallTrial", "MediumTrial" });

        Place(room);
    }

    [MenuItem("GameObject/Current Project/Level", false, 0)]
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
}
