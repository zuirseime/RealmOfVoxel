using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Slider _slider;

    public void Initialize(Entity entity)
    {
        enabled = true;

        _slider.maxValue = entity.Health.MaxValue;
        _slider.value = entity.Health.Value;

        entity.HealthChanged += OnHealthChanged;
        entity.Died += OnBossDied;
    }

    private void OnHealthChanged(object sender, AttributeEventArgs args)
    {
        _slider.maxValue = args.MaxValue;
        _slider.value = args.CurrentValue;
    }

    private void OnBossDied(object sender, EventArgs args)
    {
        enabled = false;
    }
}
