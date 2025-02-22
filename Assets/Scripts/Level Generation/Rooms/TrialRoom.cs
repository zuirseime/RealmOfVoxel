using System.Collections.Generic;
using UnityEngine;

public class TrialRoom : Room
{
    [SerializeField] protected List<GameObject> _spawnArea;
    [SerializeField] protected int _enemiesNumber;
    [SerializeField] protected List<GameObject> _enemyPrefabs;
    [SerializeField] protected List<GameObject> _enemies;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void Prepare()
    {
    }

    protected override void Validate()
    {
        base.Validate();
    }
}
