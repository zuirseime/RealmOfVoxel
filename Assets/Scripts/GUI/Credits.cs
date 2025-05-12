using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hint;

    private KeyCode _acceptKey;

    void Start()
    {
        _acceptKey = Settings.Instance.Input.Accept;
        _hint.text = _acceptKey.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(_acceptKey) || Input.GetKeyDown(Settings.Instance.Input.Pause))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
