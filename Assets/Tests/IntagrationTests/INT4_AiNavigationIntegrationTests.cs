using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class INT4_AiNavigationIntegrationTests
{
    private Game game;
    private Player player;
    private DummyEnemy enemy;
    private GameObject surfaceGO;
    private LevelGenerator levelGenerator;

    [SetUp]
    public void SetUp()
    {
        surfaceGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
        surfaceGO.transform.localScale = new Vector3(100, 1, 100);
        var surface = surfaceGO.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        TestHelper.LoadResource(out game, "Game");
        TestHelper.InvokePrivateMethod(game, "Awake");

        TestHelper.LoadResource(out player, "Entities/Player");
        TestHelper.SetEntityModifiers(player);

        TestHelper.LoadResource(out enemy, "Entities/Enemies/Enemy");
        enemy.Health.Value = 10;
        TestHelper.InvokePrivateMethod(enemy, "Awake");
        enemy.Activate();

        enemy.transform.position = Vector3.forward * 20;
    }

    private void MovePlayerRelativeToDetectionRange(float factor)
    {
        player.transform.position = enemy.transform.position + Vector3.back * (enemy.DetectionRange * factor);
    }

    [UnityTest]
    public IEnumerator INT4_01_PlayerEntersDetectionRange_EnemyStartsChasing()
    {
        Assert.IsNull(enemy.Target, "Enemy target should be null initially.");
        enemy.StateChanged += (sender, args) =>
        {
            var state = TestHelper.GetPrivateFieldValue(enemy, "_currentState");
            Assert.IsInstanceOf<DummyEnemyChaseState>(state, "Enemy should be in Chase state.");
        };

        MovePlayerRelativeToDetectionRange(0.8f);
        yield return new WaitForSeconds(0.2f);

        Assert.IsNotNull(enemy.Target, "Enemy target should be set to the player.");
        Assert.AreEqual(player, enemy.Target, "Enemy target should be the correct player object.");
        Assert.IsTrue(enemy.Agent.hasPath, "NavMeshAgent should have a path.");
        Assert.Greater(enemy.Agent.velocity.magnitude, 0.1f, "Enemy should be moving towards player.");
    }

    [UnityTest]
    public IEnumerator INT4_02_PlayerExitsDetectionRange_EnemyStopsChasing()
    {
        MovePlayerRelativeToDetectionRange(0.8f);
        yield return new WaitForSeconds(0.2f);
        Assert.IsNotNull(enemy.Target, "Precondition failed: Enemy should be chasing.");
        Assert.IsTrue(enemy.Agent.hasPath);
        enemy.StateChanged += (sender, args) =>
        {
            var state = TestHelper.GetPrivateFieldValue(enemy, "_currentState");
            Assert.IsInstanceOf<DummyEnemyWanderState>(state, "Enemy should be in Chase state.");
        };

        MovePlayerRelativeToDetectionRange(2f);
        yield return new WaitForSeconds(0.2f);

        Assert.IsNull(enemy.Target, "Enemy target should become null after player exits range.");
    }

    [Test]
    public void INT4_03_LevelGeneration()
    {
        TestHelper.LoadResource(out levelGenerator, "Level");
        TestHelper.InvokePrivateMethod(levelGenerator, "Awake");
        levelGenerator.LevelGenerated += (sender, args) =>
        {
            Assert.IsTrue(levelGenerator.Rooms.Count > 0, "Level should have rooms.");
            foreach (var room in levelGenerator.Rooms)
            {
                Assert.IsNotNull(room, "Room should not be null.");
                Assert.IsTrue(levelGenerator.IsLevelFullyConnected(), "Level should be fully connected.");
            }
        };
        TestHelper.InvokePrivateMethod(levelGenerator, "Start");
    }
}
