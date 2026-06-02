using UnityEngine;

public class FlashlightEffect : MonoBehaviour
{
    [Header("Bob")]
    [SerializeField] private float bobSpeed = 1.5f;
    [SerializeField] private float bobIntensityAmount = 0.05f;

    [Header("Subtle Flicker")]
    [SerializeField] private float subtleNoiseSpeed = 8f;
    [SerializeField] private float subtleNoiseAmount = 0.04f;

    [Header("Occasional Flicker")]
    [SerializeField] [Range(0f, 0.02f)] private float occasionalFlickerChance = 0.004f;
    [SerializeField] private float occasionalFlickerDuration = 0.25f;

    [Header("Scary Flicker")]
    [SerializeField] [Range(0f, 0.005f)] private float scaryFlickerChance = 0.0004f;
    [SerializeField] private float scaryFlickerDuration = 1.8f;

    private Light spotLight;
    private float baseIntensity;
    private bool isFlickering;
    private bool isScary;
    private float flickerEndTime;

    void Awake()
    {
        spotLight = GetComponent<Light>();
        if (spotLight == null)
            spotLight = GetComponentInChildren<Light>(true);
    }

    void Start()
    {
        if (spotLight != null)
            baseIntensity = spotLight.intensity;
    }

    void Update()
    {
        if (spotLight == null || !spotLight.enabled)
        {
            isFlickering = false;
            return;
        }

        float intensity = baseIntensity;

        // gentle sine bob
        intensity += Mathf.Sin(Time.time * bobSpeed) * bobIntensityAmount;

        if (!isFlickering)
        {
            // continuous subtle perlin noise
            intensity += (Mathf.PerlinNoise(Time.time * subtleNoiseSpeed, 0f) - 0.5f) * subtleNoiseAmount;

            if (Random.value < scaryFlickerChance)
            {
                isFlickering = true;
                isScary = true;
                flickerEndTime = Time.time + scaryFlickerDuration;
            }
            else if (Random.value < occasionalFlickerChance)
            {
                isFlickering = true;
                isScary = false;
                flickerEndTime = Time.time + occasionalFlickerDuration;
            }
        }
        else if (Time.time >= flickerEndTime)
        {
            isFlickering = false;
        }
        else if (isScary)
        {
            float t = Time.time * 50f;
            intensity = Mathf.Sin(t) > Random.Range(-0.4f, 0.4f) ? baseIntensity : Random.Range(0f, baseIntensity * 0.2f);
        }
        else
        {
            intensity = Mathf.Sin(Time.time * 25f) > 0.1f ? baseIntensity : baseIntensity * Random.Range(0.2f, 0.5f);
        }

        spotLight.intensity = Mathf.Max(0f, intensity);
    }
}
