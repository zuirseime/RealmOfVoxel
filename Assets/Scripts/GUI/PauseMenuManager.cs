using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;

    private bool _isPaused = false;

    private void Start()
    {
        if (_pauseMenuUI != null)
        {
            _pauseMenuUI.SetActive(false);
        } else
        {
            Debug.LogError("Pause Menu UI is not assigned in the inspector.");
        }
        _isPaused = false;
        Time.timeScale = 1f;

        _resumeButton.onClick.AddListener(ResumeGame);
        _mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        if (_pauseMenuUI == null)
            return;

        _isPaused = true;

        Time.timeScale = 0f;

        _pauseMenuUI.SetActive(true);
    }

    private void ResumeGame()
    {
        if (_pauseMenuUI == null)
            return;

        _isPaused = false;

        Time.timeScale = 1f;

        _pauseMenuUI.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        var coins = FindObjectOfType<Wallet>().Coins;
        MoneyManager.ConvertCoinsToMoney(Mathf.RoundToInt(coins));

        SceneManager.LoadScene(_mainMenuSceneName);
    }
}