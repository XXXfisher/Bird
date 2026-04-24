using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscMenuManager : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject escHintText;   // 左上角【Esc】字样
    public GameObject escPanel;      // ESC 打开的总面板

    [Header("按钮")]
    public Button homeButton;
    public Button audioUpButton;
    public Button audioDownButton;

    [Header("音量设置")]
    [Range(0f, 1f)]
    public float volumeStep = 0.1f;

    [Header("主菜单场景名")]
    public string homeSceneName = "MainMenu";

    private bool isOpen = false;

    void Start()
    {
        CloseMenu();

        if (homeButton != null)
            homeButton.onClick.AddListener(GoHome);

        if (audioUpButton != null)
            audioUpButton.onClick.AddListener(AudioUp);

        if (audioDownButton != null)
            audioDownButton.onClick.AddListener(AudioDown);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
        isOpen = true;

        if (escPanel != null)
            escPanel.SetActive(true);

        if (escHintText != null)
            escHintText.SetActive(false);

        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        isOpen = false;

        if (escPanel != null)
            escPanel.SetActive(false);

        if (escHintText != null)
            escHintText.SetActive(true);

        Time.timeScale = 1f;
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(homeSceneName);
    }

    public void AudioUp()
    {
        AudioListener.volume = Mathf.Clamp01(AudioListener.volume + volumeStep);
    }

    public void AudioDown()
    {
        AudioListener.volume = Mathf.Clamp01(AudioListener.volume - volumeStep);
    }
}