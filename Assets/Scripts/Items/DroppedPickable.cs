using UnityEngine;

public class DroppedPickable : MonoBehaviour
{
    private float dropTime;
    private bool grounded;
    private Rigidbody rb;
    private const float cooldown = 0.3f;

    void Start()
    {
        dropTime = Time.time;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!grounded) return;
        if (Time.time - dropTime < cooldown) return;

        gameObject.tag = "Pickable";
        Destroy(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Floor")) return;
        if (grounded) return;
        grounded = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
