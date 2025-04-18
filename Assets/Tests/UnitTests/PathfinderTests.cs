using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathfinderTests
{
    private List<RoomBounds> bounds;
    private Pathfinder pathfinder;
    private Door startDoor;
    private Door endDoor;
    private GameObject startDoorGO, endDoorGO;
    private GameObject blockingBoundsGO;

    [SetUp]
    public void SetUp()
    {
        bounds = new List<RoomBounds>();

        startDoorGO = new GameObject("StartDoor");
        startDoorGO.transform.position = Vector3.zero;
        startDoorGO.transform.forward = Vector3.forward;
        startDoor = startDoorGO.AddComponent<Door>();

        endDoorGO = new GameObject("EndDoor");
        endDoorGO.transform.position = new Vector3(0, 0, 5);
        endDoorGO.transform.forward = Vector3.back;
        endDoor = endDoorGO.AddComponent<Door>();

        startDoor.ConnectedDoor = endDoor;
        endDoor.ConnectedDoor = startDoor;

        pathfinder = new Pathfinder(bounds, 1f);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(startDoorGO);
        UnityEngine.Object.DestroyImmediate(endDoorGO);
        if (blockingBoundsGO != null)
            UnityEngine.Object.DestroyImmediate(blockingBoundsGO);
        bounds.Clear();
    }

    private void CreateBlockingBounds(Vector3 center, Vector3 size)
    {
        blockingBoundsGO = new GameObject("BlockingBounds");
        blockingBoundsGO.transform.position = center;
        var boxCollider = blockingBoundsGO.AddComponent<BoxCollider>();
        boxCollider.size = size;
        var roomBounds = blockingBoundsGO.AddComponent<RoomBounds>();
        roomBounds.type = RoomBounds.BoundsType.Inner;
        bounds.Add(roomBounds);

        pathfinder = new Pathfinder(bounds, 1f);
    }


    [Test] // UT5_01
    public void AStarSearch_FindsPath_WhenPathExistsAndUnblocked()
    {
        Assert.IsNotNull(startDoor.ConnectedDoor);

        List<PathNode> path = pathfinder.FindPath(startDoor);

        Assert.IsNotNull(path);
        Assert.Greater(path.Count, 0, "Path should contain nodes.");
        Assert.AreEqual(Vector3Int.RoundToInt(startDoor.transform.position), path.First().Position, "Path should start at the start door.");
        Assert.AreEqual(Vector3Int.RoundToInt(endDoor.transform.position), path.Last().Position, "Path should end at the end door.");
        
        Assert.IsTrue(path.All(node => Mathf.Approximately(node.Position.x, 0)), "Path should be straight along Z axis.");
    }

    [Test] // UT5_02
    public void AStarSearch_ReturnsEmptyList_WhenPathIsBlocked()
    {
        CreateBlockingBounds(new Vector3(0, 0, 2.5f), new Vector3(5, 1, 5));

        List<PathNode> path = pathfinder.FindPath(startDoor);

        Assert.IsNotNull(path);
        Assert.AreEqual(0, path.Count, "Path should be empty when blocked.");
    }

    [Test] // UT5_03
    public void FindPath_ReturnsEmptyList_WhenEndDoorNotConnected()
    {
        startDoor.ConnectedDoor = null;

        List<PathNode> path = pathfinder.FindPath(startDoor);

        Assert.IsNotNull(path);
        Assert.AreEqual(0, path.Count, "Path should be empty when end door is not connected.");
    }

    [Test] // UT5_04
    public void IsBlocked_ReturnsTrue_WhenPositionIsInsideInnerBounds()
    {
        CreateBlockingBounds(new Vector3(1, 0, 1), new Vector3(1, 1, 1));
        
        Vector3 testPosition = new Vector3(1, 0, 1);

        var isBlockedMethod = typeof(Pathfinder).GetMethod("IsBlocked", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isBlockedMethod == null)
            Assert.Inconclusive("Cannot find IsBlocked method.");

        bool result = (bool)isBlockedMethod.Invoke(pathfinder, new object[] { testPosition });

        Assert.IsTrue(result, "Position inside inner bounds should be blocked.");
    }

    [Test] // UT5_05
    public void IsBlocked_ReturnsFalse_WhenPositionIsOutsideInnerBounds()
    {
        CreateBlockingBounds(new Vector3(1, 0, 1), new Vector3(1, 1, 1));
        
        Vector3 testPosition = new Vector3(3, 0, 3);

        var isBlockedMethod = typeof(Pathfinder).GetMethod("IsBlocked", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isBlockedMethod == null)
            Assert.Inconclusive("Cannot find IsBlocked method.");

        bool result = (bool)isBlockedMethod.Invoke(pathfinder, new object[] { testPosition });

        Assert.IsFalse(result, "Position outside inner bounds should not be blocked.");
    }

    [Test]
    public void IsBlocked_ReturnsFalse_WhenPositionIsInsideOuterBounds()
    {
        GameObject outerBoundsGO = new GameObject("OuterBounds");
        outerBoundsGO.transform.position = new Vector3(1, 0, 1);
        var boxCollider = outerBoundsGO.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(1, 1, 1);
        var roomBounds = outerBoundsGO.AddComponent<RoomBounds>();
        roomBounds.type = RoomBounds.BoundsType.Outer;
        bounds.Add(roomBounds);
        pathfinder = new Pathfinder(bounds, 1f);

        Vector3 testPosition = new Vector3(1, 0, 1);

        var isBlockedMethod = typeof(Pathfinder).GetMethod("IsBlocked", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isBlockedMethod == null)
            Assert.Inconclusive("Cannot find IsBlocked method.");

        bool result = (bool)isBlockedMethod.Invoke(pathfinder, new object[] { testPosition });

        Assert.IsFalse(result, "Position inside outer bounds should not be blocked.");
        UnityEngine.Object.DestroyImmediate(outerBoundsGO);
    }


    [Test] // UT5_06
    public void Heuristic_ReturnsCorrectDistance()
    {
        Vector3 pointA = Vector3.zero;
        Vector3 pointB = new Vector3(3, 0, 4);
        float expectedDistance = 5f;

        var heuristicMethod = typeof(Pathfinder).GetMethod("Heuristic", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (heuristicMethod == null)
            Assert.Inconclusive("Cannot find Heuristic method.");

        float result = (float)heuristicMethod.Invoke(pathfinder, new object[] { pointA, pointB });

        Assert.AreEqual(expectedDistance, result, 0.01f);
    }
}