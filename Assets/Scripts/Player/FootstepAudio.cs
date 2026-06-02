using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepDistance = 1.5f;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.8f;
    [SerializeField] [Range(0f, 0.5f)] private float pitchVariation = 0.1f;

    [SerializeField] private CharacterController characterController;

    private AudioSource audioSource;
    private float stepTimer;
    private int lastClipIndex = -1;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float speed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;
        bool grounded = characterController.isGrounded;
        bool moving = speed > 0.1f && grounded;

        if (!moving)
            return;

        stepTimer += Time.deltaTime;
        float interval = stepDistance / speed;

        if (stepTimer >= interval)
        {
            stepTimer = 0f;
            PlayStep();
        }
    }

    void PlayStep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        int index;
        do { index = Random.Range(0, footstepClips.Length); }
        while (footstepClips.Length > 1 && index == lastClipIndex);
        lastClipIndex = index;

        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(footstepClips[index], volume);
    }
}
