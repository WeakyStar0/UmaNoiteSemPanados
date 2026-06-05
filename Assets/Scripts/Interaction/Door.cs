using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{
    [Header("Lock")]
    [SerializeField] private string requiredKeyId;
    [SerializeField] private bool isLocked = true;

    [Header("Opening")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private float closeSpeedDegrees = 180f;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 3f;

    [Header("Open Direction Zones")]
    [SerializeField] private Vector3 sideAOffset = new Vector3(0f, 0f, 1f);
    [SerializeField] private Vector3 sideBOffset = new Vector3(0f, 0f, -1f);
    [SerializeField] private float zoneGizmoRadius = 0.25f;

    [Header("Audio")]
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] [Range(0f, 1f)] private float lockedSoundVolume = 1f;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField] [Range(0f, 1f)] private float unlockSoundVolume = 1f;
    [SerializeField] private AudioClip openSound;
    [SerializeField] [Range(0f, 1f)] private float openSoundVolume = 1f;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] [Range(0f, 1f)] private float closeSoundVolume = 1f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI lockedText;
    [SerializeField] private float lockedTextDuration = 2f;

    private AudioSource audioSource;
    private Transform playerTransform;
    private CharacterController playerCC;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion targetRotation;
    private bool isOpen = false;
    private bool isAnimating = false;
    private bool isClosing = false;
    private bool closeSoundPlayed = false;
    private float lockedTextTimer = 0f;
    private float lastInteractTime = -Mathf.Infinity;
    private const float interactCooldown = 0.1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0.5f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 10f;
        audioSource.playOnAwake = false;

        closedRotation = transform.localRotation;
        targetRotation = closedRotation;
        playerTransform = Camera.main.transform;

        playerCC = Camera.main.transform.root.GetComponent<CharacterController>();
        if (playerCC == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerCC = player.GetComponent<CharacterController>();
        }

        if (lockedText != null)
            lockedText.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(requiredKeyId))
            isLocked = false;
    }

    void Update()
    {
        if (lockedTextTimer > 0f)
        {
            lockedTextTimer -= Time.deltaTime;
            if (lockedTextTimer <= 0f && lockedText != null)
                lockedText.gameObject.SetActive(false);
        }

        bool paused = PauseManager.Instance != null && (PauseManager.Instance.IsPaused || PauseManager.Instance.IsJustResumed);
        bool eKey = !paused && Keyboard.current.eKey.wasPressedThisFrame;

        if (eKey && IsPlayerLookingAtDoor() && Time.time - lastInteractTime >= interactCooldown)
        {
            lastInteractTime = Time.time;
            if (isLocked)
            {
                PlaySound(lockedSound, lockedSoundVolume);
                ShowLockedText();
            }
            else
            {
                if (isOpen) Close();
                else Open();
            }
        }

        if (isAnimating)
        {
            if (isClosing)
            {
                transform.localRotation = Quaternion.RotateTowards(
                    transform.localRotation, targetRotation, closeSpeedDegrees * Time.deltaTime);
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(
                    transform.localRotation, targetRotation, Time.deltaTime * openSpeed);
            }

            if (isClosing && !closeSoundPlayed)
            {
                float remaining = Quaternion.Angle(transform.localRotation, closedRotation);
                if (remaining < openAngle * 0.01f)
                {
                    PlaySound(closeSound, closeSoundVolume);
                    closeSoundPlayed = true;
                }
            }

            if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.5f)
            {
                transform.localRotation = targetRotation;
                isAnimating = false;
                isClosing = false;
            }
        }
    }

    public bool TryUnlock(string keyId)
    {
        if (!IsPlayerGrounded()) return false;

        if (!isLocked)
        {
            Open();
            return true;
        }

        if (keyId == requiredKeyId)
        {
            isLocked = false;
            lockedTextTimer = 0f;
            if (lockedText != null) lockedText.gameObject.SetActive(false);
            PlaySound(unlockSound, unlockSoundVolume);
            Open();
            return true;
        }

        PlaySound(lockedSound, lockedSoundVolume);
        ShowLockedText();
        return false;
    }

    void Open()
    {
        isOpen = true;
        isAnimating = true;
        isClosing = false;
        closeSoundPlayed = false;

        Vector3 playerPos = playerCC != null ? playerCC.transform.position : playerTransform.position;
        Vector3 worldA = transform.TransformPoint(sideAOffset);
        Vector3 worldB = transform.TransformPoint(sideBOffset);
        float angle = Vector3.Distance(playerPos, worldA) <= Vector3.Distance(playerPos, worldB)
            ? -openAngle : openAngle;
        openRotation = closedRotation * Quaternion.Euler(0f, angle, 0f);

        targetRotation = openRotation;
        PlaySound(openSound, openSoundVolume);
    }

    void Close()
    {
        isOpen = false;
        isAnimating = true;
        isClosing = true;
        closeSoundPlayed = false;
        targetRotation = closedRotation;
    }

    bool IsPlayerLookingAtDoor()
    {
        Ray ray = new Ray(playerTransform.position, playerTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            return hit.collider.GetComponentInParent<Door>() == this;
        return false;
    }

    public bool IsLocked => isLocked;

    bool IsPlayerGrounded()
    {
        return playerCC != null && playerCC.isGrounded;
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    void ShowLockedText()
    {
        if (lockedText == null) return;
        lockedText.gameObject.SetActive(true);
        lockedText.text = "Door Locked";
        lockedTextTimer = lockedTextDuration;
    }

    void OnDrawGizmosSelected()
    {
        // interact range sphere
        Color col = isLocked ? Color.red : Color.green;
        Gizmos.color = new Color(col.r, col.g, col.b, 0.1f);
        Gizmos.DrawSphere(transform.position, interactRange);
        Gizmos.color = col;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        // side A — cyan: player here → door opens away from A
        Vector3 worldA = transform.TransformPoint(sideAOffset);
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawSphere(worldA, zoneGizmoRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(worldA, zoneGizmoRadius);

        // side B — magenta: player here → door opens away from B
        Vector3 worldB = transform.TransformPoint(sideBOffset);
        Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
        Gizmos.DrawSphere(worldB, zoneGizmoRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(worldB, zoneGizmoRadius);
    }
}
