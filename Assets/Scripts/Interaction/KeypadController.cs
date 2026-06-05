using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class KeypadController : MonoBehaviour
{
    [Header("Keypad")]
    public string KeypadId;
    public string CorrectCode;
    [Min(1)] public int MaxLength = 5;

    [Header("Lights")]
    [Tooltip("Off by default — turns on when code is correct")]
    public Light CorrectLight;
    [Tooltip("On by default — turns off when code is correct")]
    public Light WrongLight;

    [Header("Audio")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] [Range(0f, 1f)] private float buttonClickVolume = 1f;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] [Range(0f, 1f)] private float correctVolume = 1f;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] [Range(0f, 1f)] private float wrongVolume = 1f;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private ReticleUI reticleUI;

    [Header("Events")]
    public UnityEvent OnCorrect;

    private string _currentInput = "";
    private bool _solved = false;
    private bool _inputLocked = false;
    private bool _showingHint = false;
    private KeypadButton[] _buttons;
    private Camera _cam;
    private AudioSource _audioSource;

    void Start()
    {
        _cam = Camera.main;
        _buttons = GetComponentsInChildren<KeypadButton>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.spatialBlend = 1f;
        _audioSource.playOnAwake = false;

        if (CorrectLight != null) CorrectLight.enabled = false;
        if (WrongLight != null)
        {
            WrongLight.color = Color.red;
            WrongLight.enabled = true;
        }
    }

    // LateUpdate so reticle call wins over PickupManager's Update
    void LateUpdate()
    {
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        KeypadButton hovered = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            hovered = hit.collider.GetComponent<KeypadButton>();

        if (hovered != null)
        {
            reticleUI?.ShowHint(_solved ? "Unlocked" : $"[Click] {hovered.Key}");
            _showingHint = true;

            bool blocked = PauseManager.Instance != null && (PauseManager.Instance.IsPaused || PauseManager.Instance.IsJustResumed);
            if (!_solved && !_inputLocked && !blocked && Mouse.current.leftButton.wasPressedThisFrame)
                hovered.Press();
        }
        else if (_showingHint)
        {
            reticleUI?.HidePickup();
            _showingHint = false;
        }
    }

    public void PressKey(char key)
    {
        if (_solved || _inputLocked) return;

        PlaySound(buttonClickSound, buttonClickVolume);

        if (key == '*')
        {
            if (_currentInput.Length > 0)
                _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
            return;
        }

        if (key == '#')
        {
            _currentInput = "";
            return;
        }

        if (_currentInput.Length >= MaxLength) return;
        _currentInput += key;

        if (_currentInput.Length == MaxLength)
            ValidateCode();
    }

    void ValidateCode()
    {
        if (_currentInput == CorrectCode)
        {
            _solved = true;
            if (CorrectLight != null) CorrectLight.enabled = true;
            if (WrongLight != null) WrongLight.enabled = false;
            PlaySound(correctSound, correctVolume);
            OnCorrect?.Invoke();
        }
        else
        {
            _currentInput = "";
            PlaySound(wrongSound, wrongVolume);
            StartCoroutine(FailRoutine());
        }
    }

    IEnumerator FailRoutine()
    {
        _inputLocked = true;
        foreach (var btn in _buttons)
            btn.TriggerFailBlink();

        yield return new WaitForSeconds(3 * (0.15f + 0.12f));
        _inputLocked = false;
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null) return;
        _audioSource.PlayOneShot(clip, volume);
    }
}
