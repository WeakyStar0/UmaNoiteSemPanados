using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private GameObject fpsControllerObject;
    
    private bool isPaused = false;
    
    void Awake() => Instance = this;
    
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Toggle();
    }
    
    public void Toggle()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        settingsCanvas.enabled = isPaused;
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        fpsControllerObject.SetActive(!isPaused);
    }
}