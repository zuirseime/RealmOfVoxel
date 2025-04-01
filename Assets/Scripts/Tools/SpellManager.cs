using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class SpellManager : MonoBehaviour
{
    private static string path => Path.Combine(Application.persistentDataPath, "spell_data.json");

    private Dictionary<string, SpellData> _spellDataDict;
    private Spell[] _allSpells;

    private void Awake()
    {
        LoadSpells();
        LoadSpellData();
    }

    private void LoadSpells()
    {
        _allSpells = Resources.LoadAll<Spell>("Spells");
        if (_allSpells == null || _allSpells.Length == 0)
        {
            Debug.LogError("No spells found in Resources/Spells/");
        }

        foreach (var spell in _allSpells) Debug.Log(spell);
    }

    private void LoadSpellData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            List<SpellData> spellDataList = JsonConvert.DeserializeObject<List<SpellData>>(json) ?? new();
            _spellDataDict = spellDataList.ToDictionary(s => s.SpellId);

            Debug.Log(json);
        } else
        {
            _spellDataDict = new Dictionary<string, SpellData>();
        }

        foreach (var spell in _allSpells)
        {
            if (!_spellDataDict.ContainsKey(spell.ID))
            {
                _spellDataDict[spell.ID] = new SpellData() { SpellId = spell.ID, Acquired = false, Active = false };
            }
        }
    }

    public void SaveSpellData()
    {
        string json = JsonConvert.SerializeObject(_spellDataDict.Values.ToList(), Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log($"Spells saved in: {path}");
    }

    public Spell GetSpell(string id)
    {
        return _allSpells.First(s => s.ID == id);
    }

    public SpellData GetSpellData(string id)
    {
        return _spellDataDict.TryGetValue(id, out var value) ? value : null;
    }

    public void SetSpellAcquired(string id, bool acquired)
    {
        if (_spellDataDict.TryGetValue(id, out var value))
        {
            value.Acquired = acquired;
            SaveSpellData();
        }
    }

    public void SetSpellActive(string id, bool active)
    {
        if (_spellDataDict.TryGetValue(id, out var value))
        {
            value.Active = active;
            SaveSpellData();
        }
    }

    public Spell[] GetAcquired()
    {
        var acquired = _allSpells?.Where(s => GetSpellData(s.ID) is SpellData data && data.Acquired).ToArray() ?? new Spell[0];

        string result = string.Empty;
        foreach (var spell in acquired)
            result += spell.ToString() + "\t";

        return acquired;
    }

    public Spell[] GetActive()
    {
        var active = GetAcquired().Where(s => GetSpellData(s.ID) is SpellData data && data.Active).ToArray();

        string result = string.Empty;
        foreach (var spell in active)
            result += spell.ToString() + "\t";

        return active;
    }
}