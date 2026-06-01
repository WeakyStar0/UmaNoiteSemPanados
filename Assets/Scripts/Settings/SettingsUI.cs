// Assets/Scripts/Settings/SettingsUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider fpsSlider;
    [SerializeField] private Toggle vsyncToggle;
    
    void Start()
    {
        LoadSettingsUI();
        SetupListeners();
    }
    
    void LoadSettingsUI()
    {
        var s = SettingsManager.Instance.Settings;
        masterVolumeSlider.value = s.MasterVolume;
        musicVolumeSlider.value = s.MusicVolume;
        sfxVolumeSlider.value = s.SFXVolume;
        qualityDropdown.value = s.GraphicsQuality;
        fullscreenToggle.isOn = s.Fullscreen;
        fpsSlider.value = s.MaxFPS;
        vsyncToggle.isOn = s.VSync;
    }
    
    void SetupListeners()
    {
        masterVolumeSlider.onValueChanged.AddListener(v => SettingsManager.Instance.Settings.MasterVolume = v);
        musicVolumeSlider.onValueChanged.AddListener(v => SettingsManager.Instance.Settings.MusicVolume = v);
        sfxVolumeSlider.onValueChanged.AddListener(v => SettingsManager.Instance.Settings.SFXVolume = v);
        qualityDropdown.onValueChanged.AddListener(v => SettingsManager.Instance.Settings.GraphicsQuality = v);
        fullscreenToggle.onValueChanged.AddListener(v => SettingsManager.Instance.Settings.Fullscreen = v);
        fpsSlider.onValueChanged.AddListener(v => SettingsManager.Instance.Settings.MaxFPS = (int)v);
        vsyncToggle.onValueChanged.AddListener(v => SettingsManager.Instance.Settings.VSync = v);
    }
    
    public void ShowAudioPanel()
    {
        audioPanel.SetActive(true);
        videoPanel.SetActive(false);
    }
    
    public void ShowVideoPanel()
    {
        audioPanel.SetActive(false);
        videoPanel.SetActive(true);
    }
    
    public void ApplySettings() => SettingsManager.Instance.ApplySettings();
    public void SaveSettings() => SettingsManager.Instance.SaveSettings();
}