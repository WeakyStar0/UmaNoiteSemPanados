// Assets/Scripts/Settings/GameSettings.cs
using UnityEngine;

public class GameSettings
{
    public float MasterVolume { get; set; } = 1f;
    public float MusicVolume { get; set; } = 0.7f;
    public float SFXVolume { get; set; } = 0.8f;
    
    public int Resolution { get; set; } = 0;
    public int GraphicsQuality { get; set; } = 2;
    public bool Fullscreen { get; set; } = true;
    public int MaxFPS { get; set; } = 60;
    public bool VSync { get; set; } = true;
}