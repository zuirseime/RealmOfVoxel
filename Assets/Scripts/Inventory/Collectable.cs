using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CapsuleCollider), typeof(NavMeshModifier))]
public abstract class Collectable : MonoBehaviour, IDisplayable
{
    private Player _playerWithinReach;

    [SerializeField] private Canvas[] _canvas;
    [SerializeField] private TextMeshProUGUI _hint;
    [SerializeField] protected bool _taken = false;

    [Header("Information")]
    [SerializeField] protected string _name;
    [SerializeField, TextArea] protected string _description;

    [Header("Status")]
    [SerializeField] protected int _price;

    [Header("Visualisation")]
    [SerializeField] protected Sprite _sprite;

    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }
    public string Title
    {
        get => _name;
        set => _name = value;
    }
    public string Description
    {
        get => _description;
        set => _description = value;
    }
    public int Price => _price;

    public Dictionary<string, string> Stats { get; set; } = new();

    public virtual void Collect(Player player)
    {
        _taken = true;
        _playerWithinReach = null;
        foreach (var c in _canvas)
        {
            c.enabled = false;
        }
    }

    protected virtual void Awake()
    {
        _canvas = GetComponentsInChildren<Canvas>();
        foreach (var c in _canvas)
        {
            c.enabled = false;
        }
    }

    protected virtual void Start()
    {
        _taken = false;
        if (_hint != null)
        _hint.text = Settings.Instance.Input.Interact.ToString();
    }

    protected virtual void Update()
    {
        foreach (var c in _canvas)
        {
            c.gameObject.SetActive(!_taken);
        }

        if (_playerWithinReach != null)
        {
            if (Input.GetKeyDown(Settings.Instance.Input.Interact))
            {
                Collect(_playerWithinReach);
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!_taken && other.TryGetComponent(out Player player))
        {
            foreach (var c in _canvas)
            {
                c.enabled = true;
            }
            _playerWithinReach = player;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            foreach (var c in _canvas)
            {
                c.enabled = false;
            }
            _playerWithinReach = null;
        }
    }

    protected void AddToStats(string key, float value, char units)
        => AddToStats(key, value, units.ToString());

    protected void AddToStats(string key, float value, string units = "")
    {
        if (value != 0)
        {
            string formattedValue = $"{Math.Round(value, 1)}{units}";
            if (!Stats.ContainsKey(key))
                Stats.Add(key, formattedValue);
            else
                Stats[key] = formattedValue;
        }
    }
}