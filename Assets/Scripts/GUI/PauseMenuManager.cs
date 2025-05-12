using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _mainMenuButton;

    [SerializeField] private SettingsMenu _settingsMenu;

    private bool _isPaused = false;

    public void ChangeState(bool active, float timeScale)
    {
        if (_pauseMenuUI == null)
            return;

        _isPaused = active;

        Time.timeScale = timeScale;

        _pauseMenuUI.SetActive(active);
    }

    private void Start()
    {
        ChangeState(false, 1f);

        _resumeButton.onClick.AddListener(ResumeGame);
        _settingsButton.onClick.AddListener(OpenSettings);
        _mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OpenSettings()
    {
        _settingsMenu.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (_isPaused)
                ChangeState(false, 1);
            else
                ChangeState(true, 0);
    }

    private void ResumeGame()
    {
        ChangeState(false, 1);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        var coins = FindObjectOfType<Wallet>().Coins;
        MoneyManager.ConvertCoinsToMoney(Mathf.RoundToInt(coins));

        SceneManager.LoadScene(_mainMenuSceneName);
    }
}