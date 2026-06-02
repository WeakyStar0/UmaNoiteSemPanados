using UnityEngine;

public class Flashlight : Item
{
    [SerializeField] private string emissiveObjectName = "Emissive";
    [SerializeField] private bool startOn = false;
    [SerializeField] private float toggleCooldown = 0.3f;
    [SerializeField] private AudioClip onSound;
    [SerializeField] private AudioClip offSound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;
    [SerializeField] [Range(0.1f, 3f)] private float soundPitch = 1f;

    private Light spotLight;
    private Renderer emissiveRenderer;
    private bool isOn;
    private float lastToggleTime = -Mathf.Infinity;
    private Vector3 lastModelPosition;

    public override void OnModelSpawned(GameObject model)
    {
        spotLight = model.GetComponentInChildren<Light>(true);

        Transform emissiveTrans = FindChildByName(model.transform, emissiveObjectName);
        emissiveRenderer = emissiveTrans != null ? emissiveTrans.GetComponent<Renderer>() : null;
        lastModelPosition = model.transform.position;

        ApplyState();
    }

    public override void OnUse()
    {
        if (Time.time - lastToggleTime < toggleCooldown) return;

        lastToggleTime = Time.time;
        isOn = !isOn;
        ApplyState();

        AudioClip clip = isOn ? onSound : offSound;
        if (clip != null)
        {
            AudioSource camAudio = Camera.main.GetComponent<AudioSource>();
            if (camAudio == null) camAudio = Camera.main.gameObject.AddComponent<AudioSource>();
            camAudio.spatialBlend = 0f;
            camAudio.pitch = soundPitch;
            camAudio.PlayOneShot(clip, soundVolume);
        }
    }

    void ApplyState()
    {
        if (spotLight != null) spotLight.enabled = isOn;
        if (emissiveRenderer != null) emissiveRenderer.enabled = isOn;
        if (spotLight != null) lastModelPosition = spotLight.transform.position;
    }

    Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildByName(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
