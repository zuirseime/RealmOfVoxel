using System.Collections;
using TMPro;
using UnityEngine;

public class CoinsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Inventory _inventory;

    [SerializeField] private float _changingDuration;

    private Coroutine _animationCoroutine;

    private void Start()
    {
        _inventory.CoinsChanged += OnCoinsChanged;
        _text.text = _inventory.LocalCoins.ToString();
    }

    private void OnCoinsChanged(object sender, CoinsEventArgs args)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCoroutine = StartCoroutine(AnimateCoins(args.Last, args.Current));
    }

    private IEnumerator AnimateCoins(float startValue, float targetValue)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _changingDuration)
        {
            elapsedTime += Time.deltaTime;
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, elapsedTime / _changingDuration));
            _text.text = currentValue.ToString();
            yield return null;
        }
        _text.text = targetValue.ToString();
    }
}