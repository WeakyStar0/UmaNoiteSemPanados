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

    [Header("Debug - Adjust while playing")]
    [SerializeField] private Vector3 debugPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 debugRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 debugScaleOffset = Vector3.one;

    public string ItemID => itemID;
    public string DisplayName => displayName;
    public GameObject ModelPrefab => modelPrefab;
    public Vector3 HandPositionOffset => handPositionOffset;
    public Vector3 HandRotationOffset => handRotationOffset;
    public Vector3 HandScaleOffset => handScaleOffset;
    public Vector3 DebugPositionOffset => debugPositionOffset;
    public Vector3 DebugRotationOffset => debugRotationOffset;
    public Vector3 DebugScaleOffset => debugScaleOffset;

    public virtual void OnPickup()
    {
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

