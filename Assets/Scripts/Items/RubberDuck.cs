using UnityEngine;

public class RubberDuck : Item
{
    [SerializeField] private AudioClip quackSound;
    [SerializeField] private float squishCooldown = 1f;

    private Animator animator;
    private AudioSource audioSource;
    private float lastSquishTime = -Mathf.Infinity;

    public override void OnModelSpawned(GameObject model)
    {
        animator = model.GetComponentInChildren<Animator>();
        audioSource = model.GetComponentInChildren<AudioSource>();
    }

    public override void OnUse()
    {
        if (Time.time - lastSquishTime < squishCooldown) return;

        lastSquishTime = Time.time;
        if (animator != null) animator.SetTrigger("Squish");
        if (quackSound != null)
        {
            AudioSource camAudio = Camera.main.GetComponent<AudioSource>();
            if (camAudio == null) camAudio = Camera.main.gameObject.AddComponent<AudioSource>();
            camAudio.spatialBlend = 0f;
            camAudio.pitch = 1f;
            camAudio.PlayOneShot(quackSound);
        }
    }
}
