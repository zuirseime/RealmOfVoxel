using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private const string PlayedKey = "Played";

    [Header("UI References")]
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private SpellShopUI _spellShopUI;
    [SerializeField] private SpellInventoryUI _spellInventoryUI;
    [SerializeField] private GameObject _dragIconPrefab;
    [SerializeField] private Tooltip _tooltip;
    [SerializeField] private SettingsMenu _settingsMenu;

    [Header("Scenes")]
    [SerializeField] private string _levelScene;

    private GameObject _dragIconInstance;
    private SpellUIItem _draggedItemSource = null;

    public Tooltip Tooltip => _tooltip;

    void Start()
    {
        _newGameButton?.onClick.AddListener(StartNewGame);
        _continueButton?.onClick.AddListener(ContinueGame);
        _optionsButton?.onClick.AddListener(OpenOptions);
        _creditsButton?.onClick.AddListener(OpenCredits);
        _quitButton?.onClick.AddListener(QuitGame);

        if (!PlayerPrefs.HasKey(PlayedKey))
        {
            _continueButton.GetComponent<MenuButton>().enabled = false;
        }

        MoneyManager.LoadMoney();
        UpdateMoneyDisplay(MoneyManager.CurrentMoney);

        MoneyManager.OnMoneyChanged += UpdateMoneyDisplay;

        if (_spellShopUI == null) Debug.LogError("SpellShopUI is not assigned.");
        if (_spellInventoryUI == null) Debug.LogError("SpellInventoryUI is not assigned.");

        if (_dragIconInstance != null) _dragIconInstance.SetActive(false);
    }

    private void OnDestroy()
    {
        _newGameButton?.onClick.RemoveListener(StartNewGame);
        _continueButton?.onClick.RemoveListener(ContinueGame);
        _optionsButton?.onClick.RemoveListener(OpenOptions);
        _quitButton?.onClick.RemoveListener(QuitGame);

        MoneyManager.OnMoneyChanged -= UpdateMoneyDisplay;

        if (_dragIconInstance != null)
        {
            Destroy(_dragIconInstance);
        }
    }

    private void UpdateMoneyDisplay(int amount)
    {
        if (_moneyText != null)
        {
            _moneyText.text = $"{amount}";
        }
    }

    public void HandleDragStart(PointerEventData eventData, SpellUIItem sourceItem)
    {
        if (_dragIconPrefab != null && sourceItem.Spell != null)
        {
            _draggedItemSource = sourceItem;

            if (_dragIconInstance == null)
            {
                Canvas rootCanvas = GetComponentInParent<Canvas>();
                _dragIconInstance = Instantiate(_dragIconPrefab, rootCanvas.transform);
                _dragIconInstance.GetComponent<Image>().sprite = sourceItem.Spell.Sprite;
                _dragIconInstance.GetComponent<RectTransform>().sizeDelta = sourceItem.GetComponent<RectTransform>().sizeDelta * 0.8f;

                _dragIconInstance.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

            _dragIconInstance.GetComponent<Image>().sprite = sourceItem.Spell.Sprite;
            _dragIconInstance.SetActive(true);
            PositionDragIcon(eventData);
        }
    }

    private void Update()
    {
        if (_dragIconInstance != null && _dragIconInstance.activeSelf && Input.mousePresent)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _dragIconInstance.transform.parent as RectTransform,
                Input.mousePosition,
                (RenderMode.ScreenSpaceOverlay == GetComponentInParent<Canvas>().renderMode) ? null : GetComponentInParent<Canvas>()?.worldCamera,
                out Vector2 localPoint
            );

            _dragIconInstance.GetComponent<RectTransform>().localPosition = localPoint;
        }
    }

    public void HandleDragEnd(PointerEventData eventData)
    {
        if (_dragIconInstance != null)
        {
            _dragIconInstance.SetActive(false);
        }

        GameObject objectUnderPointer = eventData.pointerEnter;
        bool droppedOnInventory = false;
        if (objectUnderPointer != null && _draggedItemSource != null && !_draggedItemSource.IsInShop)
        {
            SpellUIItem potentialDropTarget = objectUnderPointer.GetComponentInParent<SpellUIItem>();
            if (potentialDropTarget != null && !potentialDropTarget.IsInShop)
            {
                droppedOnInventory = true;
            }
        }

        if (_draggedItemSource != null && !_draggedItemSource.IsInShop && !droppedOnInventory)
        {
            Debug.Log($"Spell {_draggedItemSource.Spell?.Title} dropped outside inventory. Deactivating.");
            if (Game.Instance != null && Game.Instance.SpellManager != null && _draggedItemSource.Spell != null)
            {
                Game.Instance.SpellManager.SetSpellActive(_draggedItemSource.Spell.ID, false);
                Game.Instance.Save();
                _spellInventoryUI?.RefreshInventory();
                _spellShopUI?.RefreshShopVisuals();
            } else
            {
                Debug.LogError("Null reference detected when trying to deactivate spell dropped outside inventory!");
            }
        }

        _draggedItemSource = null;
    }

    public void RefreshShopIU()
    {
        _spellShopUI?.RefreshShopVisuals();
    }

    private void StartNewGame()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Starting New Game...");

        MoneyManager.DropMoney();
        Game game = FindObjectOfType<Game>();
        game.SpellManager.ResetAll();

        PlayerPrefs.SetInt(PlayedKey, 1);

        SceneManager.LoadScene(_levelScene);
    }

    private void ContinueGame()
    {
        Debug.Log("Continuing Game...");
        SceneManager.LoadScene(_levelScene);
    }

    private void OpenOptions()
    {
        Debug.Log("Opening Settings...");
        _settingsMenu.gameObject.SetActive(true);
    }

    private void OpenCredits()
    {
        Debug.Log("Opening Credits...");

        SceneManager.LoadScene("Credits");
    }

    private void QuitGame()
    {
        FindObjectOfType<Game>()?.Save();
        Debug.Log("Quitting Game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void PositionDragIcon(PointerEventData eventData)
    {
        if (_dragIconInstance != null)
        {
            var rt = _dragIconInstance.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
            {
                rt.position = globalMousePos;
                rt.rotation = rt.parent.rotation;
            }
        }
    }
}
