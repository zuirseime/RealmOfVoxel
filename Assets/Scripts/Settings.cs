using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Settings
{
    private static Settings _instance;
    public static Settings Instance => _instance ??= new Settings();

    public AudioMixer AudioMixer { get; set; }

    private bool _fullscreen = true;
    private float _volume;
    private int _qualityIndex;
    private int _resolutionIndex;

    private Resolution[] _allResolutions;
    private List<Resolution> _resolutions = new();

    public List<Resolution> Resolutions => _resolutions;
    public bool Fullscreen
    {
        get => _fullscreen;
        set
        {
            if (_fullscreen != value)
            {
                _fullscreen = value;
                Screen.fullScreen = _fullscreen;
                PlayerPrefs.SetInt(nameof(Fullscreen), Convert.ToInt32(_fullscreen));
            }
        }
    }
    public float Volume
    {
        get => _volume;
        set
        {
            if (_volume != value)
            {
                _volume = value;
                AudioMixer?.SetFloat("volume", _volume);
                PlayerPrefs.SetFloat(nameof(Volume), _volume);
            }
        }
    }
    public int QualityIndex
    {
        get => _qualityIndex;
        set
        {
            if (_qualityIndex != value)
            {
                _qualityIndex = value;
                QualitySettings.SetQualityLevel(_qualityIndex);
                PlayerPrefs.SetInt(nameof(QualityIndex), _qualityIndex);
            }
        }
    }
    public int ResolutionIndex
    {
        get => _resolutionIndex;
        set
        {
            if (_resolutionIndex != value)
            {
                _resolutionIndex = value;
                var resolution = _resolutions[_resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Fullscreen);
                PlayerPrefs.SetInt(nameof(ResolutionIndex), _resolutionIndex);
            }
        }
    }

    private Settings() { }

    public InputSettings Input { get; private set; } = new();

    public void LoadSettings()
    {
        _resolutions.Clear();
        _allResolutions = Screen.resolutions;

        foreach (var resolution in _allResolutions)
        {
            if (!_resolutions.Any(r => r.width == resolution.width && r.height == resolution.height))
            {
                _resolutions.Add(resolution);
            }
        }

        Fullscreen = PlayerPrefs.GetInt(nameof(Fullscreen), 1) == 1;
        Volume = PlayerPrefs.GetFloat(nameof(Volume), 0f);
        QualityIndex = PlayerPrefs.GetInt(nameof(QualityIndex), 3);

        var currentResolution = _resolutions.Find(r => r.width == Screen.currentResolution.width &&
                                                       r.height == Screen.currentResolution.height);

        int currentResolutionIndex = _resolutions.IndexOf(currentResolution);
        ResolutionIndex = PlayerPrefs.GetInt(nameof(ResolutionIndex), currentResolutionIndex);
    }
}

public class InputSettings
{
    public KeyCode Pause { get; set; } = KeyCode.Escape;

    public KeyCode Spell1 { get; set; } = KeyCode.Q;
    public KeyCode Spell2 { get; set; } = KeyCode.W;
    public KeyCode Spell3 { get; set; } = KeyCode.E;
    public KeyCode Spell4 { get; set; } = KeyCode.R;

    public KeyCode SwapWeapons { get; set; } = KeyCode.Tab;

    public KeyCode Interact { get; set; } = KeyCode.F;

    public KeyCode Accept { get; set; } = KeyCode.Space;
}