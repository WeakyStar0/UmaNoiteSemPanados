using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private bool isInSettings = false;

    void Awake() => Instance = this;

    void Start()
    {
        pauseMenuCanvas.enabled = false;
        settingsCanvas.enabled = false;
        LockCursor();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isInSettings)
                CloseSettings();
            else
                Toggle();
        }
    }

    public void Toggle()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuCanvas.enabled = isPaused;
        settingsCanvas.enabled = false;
        isInSettings = false;

        if (isPaused)
        {
            UnlockCursor();
            playerInput.DeactivateInput();
        }
        else
        {
            LockCursor();
            playerInput.ActivateInput();
        }
    }

    public void Resume() { if (isPaused) Toggle(); }

    public void OpenSettings()
    {
        isInSettings = true;
        pauseMenuCanvas.enabled = false;
        settingsCanvas.enabled = true;
    }

    public void CloseSettings()
    {
        isInSettings = false;
        settingsCanvas.enabled = false;
        pauseMenuCanvas.enabled = true;
    }

    public void LoadMainMenu()
    {
        isPaused = false;
        isInSettings = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
