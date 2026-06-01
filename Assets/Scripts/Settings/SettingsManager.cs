// Assets/Scripts/Settings/SettingsManager.cs
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public GameSettings Settings { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    void LoadSettings() => Settings = new GameSettings();

    public void ApplySettings()
    {
        AudioListener.volume = Settings.MasterVolume;
        QualitySettings.SetQualityLevel(Settings.GraphicsQuality);
        Application.targetFrameRate = Settings.MaxFPS;
        Screen.fullScreen = Settings.Fullscreen;
        QualitySettings.vSyncCount = Settings.VSync ? 1 : 0;
        Debug.Log($"Settings applied: Volume={Settings.MasterVolume}, Quality={Settings.GraphicsQuality}, FPS={Settings.MaxFPS}");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", Settings.MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", Settings.MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", Settings.SFXVolume);
        PlayerPrefs.SetInt("Quality", Settings.GraphicsQuality);
        PlayerPrefs.SetInt("Fullscreen", Settings.Fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("MaxFPS", Settings.MaxFPS);
        PlayerPrefs.SetInt("VSync", Settings.VSync ? 1 : 0);
        PlayerPrefs.Save();
    }
}