using UnityEngine;

// Attached at runtime to dropped key model. Snaps flat on first ground contact then removes itself.
public class DroppedItem : MonoBehaviour
{
    public AudioClip landSound;
    public float landVolume = 0.8f;

    private Rigidbody rb;
    private bool landed = false;
    private float spawnTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        // only land on Floor layer — ignores player capsule and other colliders
        if (collision.gameObject.layer != LayerMask.NameToLayer("Floor")) return;
        if (landed) return;
        landed = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // align to surface normal so it lays flat on slopes too,
        // then random yaw so it faces different directions each drop
        if (collision.contactCount > 0)
        {
            ContactPoint contact = collision.GetContact(0);
            Quaternion surfaceAlign = Quaternion.FromToRotation(Vector3.up, contact.normal);
            // 90° on X tips the key from standing-upright to lying flat
            transform.rotation = surfaceAlign * Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
            transform.position = contact.point + contact.normal * 0.02f;
        }

        if (landSound != null)
            AudioSource.PlayClipAtPoint(landSound, transform.position, landVolume);

        Destroy(this);
    }
}
