using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutions;
    [SerializeField] private TMP_Dropdown _qualities;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private RectTransform _menuArea;

    private void Start()
    {
        Settings settings = Settings.Instance;

        _resolutions.ClearOptions();
        List<string> options = settings.Resolutions.Select(r => $"{r.width}x{r.height}").ToList();
        _resolutions.AddOptions(options);

        _qualities.SetValueWithoutNotify(settings.QualityIndex);
        _volumeSlider.SetValueWithoutNotify(settings.Volume);
        _fullscreenToggle.SetIsOnWithoutNotify(settings.Fullscreen);
        _resolutions.SetValueWithoutNotify(settings.ResolutionIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.Instance.Input.Pause))
        {
            Close();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(_menuArea, Input.mousePosition, null))
            {
                Close();
            }
        }
    }

    private void Close() => gameObject.SetActive(false);

    public void SetVolume(float volume)
    {
        Settings.Instance.Volume = volume;
    }

    public void SetQuality(int qualityIndex)
    {
        Settings.Instance.QualityIndex = qualityIndex;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Settings.Instance.Fullscreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Settings.Instance.ResolutionIndex = resolutionIndex;
    }
}
