using System.Linq;
using UnityEngine;

public class DetectionRangeManager : MonoBehaviour
{
    public Material material;
    private Enemy[] _enemies;
    private Vector4[] _enemyPositions = new Vector4[100];
    private float[] _detectionRanges = new float[100];
    private float[] _attackRanges = new float[100];

    private Player _player;
    private float _spellRange;

    private static readonly int playerPositionID = Shader.PropertyToID("_PlayerPosition");
    private static readonly int playerAttackRangeID = Shader.PropertyToID("_PlayerAttackRange");

    private static readonly int enemyPositionID = Shader.PropertyToID("_EnemyPositions");
    private static readonly int detectionRangesID = Shader.PropertyToID("_DetectionRanges");
    private static readonly int attackRangesID = Shader.PropertyToID("_AttackRanges");
    private static readonly int enemyCountID = Shader.PropertyToID("_EnemyCount");

    private static readonly int spellRangeID = Shader.PropertyToID("_SpellRange");

    private void OnEnable()
    {
        FindObjectOfType<Game>().SpellSetChanged += OnSpellSetChanged;
    }

    private void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        material.SetFloat(spellRangeID, _spellRange);

        if (_player.Inventory != null && _player.Inventory.CurrentWeapon != null)
        {
            float attackRange = _player.Inventory.CurrentWeapon.Range;

            if (_player.IsAlive)
            {
                material.SetVector(playerPositionID, _player.transform.position);
                material.SetFloat(playerAttackRangeID, attackRange);
            }
        }

        _enemies = FindObjectsOfType<Enemy>().Where(e => e.IsAlive).ToArray();

        int enemyCount = Mathf.Min(_enemies.Length, 100);

        for (int i = 0; i < enemyCount; i++)
        {
            _enemyPositions[i] = _enemies[i].transform.position;
            _detectionRanges[i] = _enemies[i].DetectionRange;
            _attackRanges[i] = _enemies[i].AttackRange;
        }

        material.SetVectorArray(enemyPositionID, _enemyPositions);
        material.SetFloatArray(detectionRangesID, _detectionRanges);
        material.SetFloatArray(attackRangesID, _attackRanges);
        material.SetInt(enemyCountID, enemyCount);
    }

    private void OnSpellSelected(object sender, SpellEventArgs args)
    {
        _spellRange = args.Spell.Range;
    }

    private void OnSpellDeselected(object sender, SpellEventArgs args)
    {
        _spellRange = 0.01f;
    }

    private void OnSpellSetChanged(object sender, SpellSetEventArgs args)
    {
        foreach (var spell in args.Spells)
        {
            if (spell != null)
            {
                spell.SpellSelected += OnSpellSelected;
                spell.SpellDeselected += OnSpellDeselected;
            }
        }
    }
}
