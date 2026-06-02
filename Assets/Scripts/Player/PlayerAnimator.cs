using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;

    [Header("Speed Thresholds")]
    [SerializeField] private float walkThreshold = 0.1f;
    [SerializeField] private float runThreshold = 5.5f;

    [Header("Smoothing")]
    [SerializeField] private float dampTime = 0.1f;

    [Header("Head Look")]
    [SerializeField] private Transform headBone;
    [SerializeField] [Range(0f, 1f)] private float headInfluence = 0.4f;

    [Header("Shadow Hand")]
    [SerializeField] private Transform handBone;

    private GameObject shadowHandModel;
    private Item lastShadowItem;

    private static readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    private static readonly int VelocityZHash = Animator.StringToHash("VelocityZ");

    void Start()
    {
        SetShadowsOnly();
    }

    void Update()
    {
        Vector3 localVel = characterController.transform.InverseTransformDirection(characterController.velocity);

        float vx = localVel.x / runThreshold;
        float vz = localVel.z / runThreshold;

        if (Mathf.Abs(vx) < walkThreshold / runThreshold) vx = 0f;
        if (Mathf.Abs(vz) < walkThreshold / runThreshold) vz = 0f;

        animator.SetFloat(VelocityXHash, vx, dampTime, Time.deltaTime);
        animator.SetFloat(VelocityZHash, vz, dampTime, Time.deltaTime);

        if (handBone != null)
            SyncShadowHand();
    }

    void SyncShadowHand()
    {
        Item current = Inventory.Instance?.GetSelectedItem();
        if (current == lastShadowItem) return;

        if (shadowHandModel != null) Destroy(shadowHandModel);
        shadowHandModel = null;
        lastShadowItem = current;

        if (current == null || current.ModelPrefab == null) return;

        shadowHandModel = Instantiate(current.ModelPrefab, handBone);
        shadowHandModel.transform.localPosition = Vector3.zero;
        shadowHandModel.transform.localRotation = Quaternion.identity;
        shadowHandModel.transform.localScale = Vector3.one;

        foreach (Renderer r in shadowHandModel.GetComponentsInChildren<Renderer>())
            r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

        foreach (Collider c in shadowHandModel.GetComponentsInChildren<Collider>())
            c.enabled = false;

        foreach (Light l in shadowHandModel.GetComponentsInChildren<Light>())
            l.enabled = false;

        Rigidbody rb = shadowHandModel.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    void LateUpdate()
    {
        if (headBone == null) return;

        float pitch = Camera.main.transform.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        headBone.localRotation *= Quaternion.Euler(pitch * headInfluence, 0f, 0f);
    }

    void SetShadowsOnly()
    {
        foreach (Renderer r in animator.GetComponentsInChildren<Renderer>())
            r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }
}
