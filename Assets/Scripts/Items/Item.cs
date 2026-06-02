using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemID;
    [SerializeField] private string displayName;
    [SerializeField] private GameObject modelPrefab;

    [Header("Hand Position")]
    [SerializeField] private Vector3 handPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 handRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 handScaleOffset = Vector3.one;

    [Header("Hotbar Preview")]
    [SerializeField] private Vector3 hotbarPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 hotbarRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 hotbarScaleOffset = Vector3.one;

    [Header("Drop")]
    [SerializeField] private bool droppable = true;
    [SerializeField] private bool lockDropRotX;
    [SerializeField] private bool lockDropRotY;
    [SerializeField] private bool lockDropRotZ;

    private Quaternion originalRotation;

    [Header("Debug - Adjust while playing")]
    [SerializeField] private Vector3 debugPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 debugRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 debugScaleOffset = Vector3.one;

    public string ItemID => itemID;
    public string DisplayName => displayName;
    public GameObject ModelPrefab => modelPrefab;
    public bool Droppable => droppable;
    public Vector3 HotbarPositionOffset => hotbarPositionOffset;
    public Vector3 HotbarRotationOffset => hotbarRotationOffset;
    public Vector3 HotbarScaleOffset => hotbarScaleOffset;
    public Vector3 HandPositionOffset => handPositionOffset;
    public Vector3 HandRotationOffset => handRotationOffset;
    public Vector3 HandScaleOffset => handScaleOffset;
    public Vector3 DebugPositionOffset => debugPositionOffset;
    public Vector3 DebugRotationOffset => debugRotationOffset;
    public Vector3 DebugScaleOffset => debugScaleOffset;

    void Awake()
    {
        originalRotation = transform.rotation;
    }

    public virtual void OnPickup() { }
    public virtual void OnModelSpawned(GameObject model) { }
    public virtual void OnUse() { }
    public virtual void OnInteract() { }

    public void Drop(Vector3 position, Vector3 velocity)
    {
        gameObject.tag = "Untagged";
        gameObject.isStatic = false;

        bool anyLocked = lockDropRotX || lockDropRotY || lockDropRotZ;

        // position before SetActive to prevent one-frame flash at old position
        transform.position = position;
        gameObject.SetActive(true);

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
            r.allowOcclusionWhenDynamic = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // build constraints before setting rotation so rb doesn't fight us
        RigidbodyConstraints constraints = RigidbodyConstraints.None;
        if (lockDropRotX) constraints |= RigidbodyConstraints.FreezeRotationX;
        if (lockDropRotY) constraints |= RigidbodyConstraints.FreezeRotationY;
        if (lockDropRotZ) constraints |= RigidbodyConstraints.FreezeRotationZ;
        rb.constraints = constraints;

        // use rb.position/rotation AFTER rigidbody is configured
        // to override the cached state restored by SetActive(true)
        rb.position = position;
        rb.rotation = anyLocked ? originalRotation : Random.rotation;
        rb.linearVelocity = velocity;
        rb.angularVelocity = anyLocked ? Vector3.zero : Random.insideUnitSphere * 4f;

        // prevent item from colliding with player capsule
        if (col != null)
        {
            CharacterController cc = Camera.main.transform.root.GetComponent<CharacterController>();
            if (cc != null) Physics.IgnoreCollision(col, cc, true);
        }

        gameObject.AddComponent<DroppedPickable>();
    }

    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.yellow;
            if (col is BoxCollider bc)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + bc.center, transform.rotation, transform.lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, bc.size);
            }
            else if (col is SphereCollider sc)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + sc.center, transform.rotation, transform.lossyScale);
                Gizmos.DrawWireSphere(Vector3.zero, sc.radius);
            }
        }
    }
}

