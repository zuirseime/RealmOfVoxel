using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Wallet wallet = player.GetComponent<Wallet>();

            MoneyManager.ConvertCoinsToMoney(Mathf.RoundToInt(wallet.Coins));
            SceneManager.LoadScene(_nextSceneName);
        }
    }
}
