using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class SpellManager : MonoBehaviour
{
    private const string SpellDataPrefsKey = "PlayerSpellData";

    //private static string path => Path.Combine(Application.persistentDataPath, "spell_data.json");

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
        //if (File.Exists(path))
        //{
        //    string json = File.ReadAllText(path);
        //    List<SpellData> spellDataList = JsonConvert.DeserializeObject<List<SpellData>>(json) ?? new();
        //    _spellDataDict = spellDataList.ToDictionary(s => s.SpellId);

        //    Debug.Log(json);
        //} else
        //{
        //    _spellDataDict = new Dictionary<string, SpellData>();
        //}

        //foreach (var spell in _allSpells)
        //{
        //    if (!_spellDataDict.ContainsKey(spell.ID))
        //    {
        //        _spellDataDict[spell.ID] = new SpellData() { SpellId = spell.ID, Acquired = false, Active = false };
        //    }
        //}

        if (PlayerPrefs.HasKey(SpellDataPrefsKey))
        {
            string json = PlayerPrefs.GetString(SpellDataPrefsKey);
            Debug.Log($"Loading Spell Data from PlayerPrefs: {json}");

            List<SpellData> spellDataList = JsonConvert.DeserializeObject<List<SpellData>>(json) ?? new List<SpellData>();

            if (spellDataList != null)
            {
                _spellDataDict = new Dictionary<string, SpellData>();
                foreach (var data in spellDataList)
                {
                    if (data != null && !string.IsNullOrEmpty(data.SpellId) && !_spellDataDict.ContainsKey(data.SpellId))
                    {
                        _spellDataDict.Add(data.SpellId, data);
                    }
                }
            } else
            {
                _spellDataDict = new Dictionary<string, SpellData>();
                Debug.LogWarning("Failed to deserialize spell data list from PlayerPrefs JSON.");
            }
        } else
        {
            Debug.Log("No Spell Data found in PlayerPrefs. Initializing new dictionary.");
            _spellDataDict = new Dictionary<string, SpellData>();
        }

        if (_allSpells != null)
        {
            foreach (var spell in _allSpells)
            {
                if (spell != null && !string.IsNullOrEmpty(spell.ID))
                {
                    if (!_spellDataDict.ContainsKey(spell.ID))
                    {
                        _spellDataDict[spell.ID] = new SpellData() { SpellId = spell.ID, Acquired = false, Active = false };
                        Debug.Log($"Added default SpellData for new spell: {spell.ID}");
                    }
                } else
                {
                    Debug.LogWarning("Found a spell with null reference or null/empty ID in _allSpells during LoadSpellData.");
                }
            }
        } else
        {
            Debug.LogError("_allSpells is null even after trying to load them in LoadSpellData.");
        }
    }

    public void SaveSpellData()
    {
        //string json = JsonConvert.SerializeObject(_spellDataDict.Values.ToList(), Formatting.Indented);
        //File.WriteAllText(path, json);
        //Debug.Log($"Spells saved in: {path}");

        if (_spellDataDict == null || _spellDataDict.Count == 0)
        {
            Debug.LogWarning("Spell data dictionary is null or empty. Nothing to save to PlayerPrefs.");
            PlayerPrefs.DeleteKey(SpellDataPrefsKey);
            PlayerPrefs.Save();
            return;
        }

        List<SpellData> spellDataList = _spellDataDict.Values.ToList();

        string json = JsonConvert.SerializeObject(spellDataList, Formatting.None);

        PlayerPrefs.SetString(SpellDataPrefsKey, json);

        PlayerPrefs.Save();

        Debug.Log($"Spells saved to PlayerPrefs under key '{SpellDataPrefsKey}'. JSON: {json}");
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

    public void SetSpellActive(string id, bool active, int slot = -1)
    {
        if (_spellDataDict.TryGetValue(id, out SpellData value))
        {
            value.Active = active;
            value.Slot = slot;

            SaveSpellData();
        }
    }

    public Spell[] GetAll() => _allSpells;

    public Spell[] GetAcquired()
    {
        var acquired = GetAll()?.Where(s => GetSpellData(s.ID) is SpellData data && data.Acquired).ToArray() ?? new Spell[0];

        return acquired;
    }

    public Spell[] GetActive()
    {
        Spell[] active = new Spell[4];

        foreach (var spell in GetAcquired().Where(s => GetSpellData(s.ID) is SpellData data && data.Active))
        {
            var slot = GetSpellData(spell.ID).Slot;
            active[slot] = spell;
        }

        return active;
    }

    public SpellData[] GetAllSpellsData()
    {
        return _spellDataDict.Values.ToArray();
    }

    public void ResetAll()
    {
        foreach (var spell in GetAll())
        {
            var spellData = GetSpellData(spell.ID);
            spellData.Acquired = false;
            spellData.Active = false;
        }
        SaveSpellData();
    }
}