using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Slider _slider;

    public void Initialize(Boss boss)
    {
        gameObject.SetActive(true);

        _name.text = boss.Name;
        _slider.maxValue = boss.Health.MaxValue;
        _slider.value = boss.Health.Value;

        boss.HealthChanged += OnHealthChanged;
        boss.Died += OnBossDied;
    }

    private void OnHealthChanged(object sender, AttributeEventArgs args)
    {
        _slider.maxValue = args.MaxValue;
        _slider.value = args.CurrentValue;
    }

    private void OnBossDied(object sender, EventArgs args)
    {
        gameObject.SetActive(false);
    }
}
