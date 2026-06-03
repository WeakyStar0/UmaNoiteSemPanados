using UnityEngine;

public class DroppedPickable : MonoBehaviour
{
    private float dropTime;
    private bool grounded;
    private Rigidbody rb;
    private const float cooldown = 0.3f;
    private const float settleStartSpeed = 1.5f;
    private const float settleEndSpeed = 0.04f;
    private const float maxSettleTime = 4f;
    private const float dragRampSpeed = 18f;
    private const float initialAngularDamping = 6f;
    private const float maxAngularDamping = 20f;

    void Start()
    {
        dropTime = Time.time;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!grounded) return;

        float elapsed = Time.time - dropTime;
        float speed = rb != null ? rb.linearVelocity.magnitude : 0f;

        if (rb != null && speed < settleStartSpeed)
            rb.angularDamping = Mathf.MoveTowards(rb.angularDamping, maxAngularDamping, Time.deltaTime * dragRampSpeed);

        bool settled = rb == null || speed < settleEndSpeed;
        bool timedOut = elapsed > maxSettleTime;

        if (!settled && !timedOut) return;
        if (elapsed < cooldown) return;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        gameObject.tag = "Pickable";
        Destroy(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (layer != LayerMask.NameToLayer("Floor") && layer != LayerMask.NameToLayer("Furniture")) return;
        grounded = true;
        if (rb != null) rb.angularDamping = initialAngularDamping;
    }
}
