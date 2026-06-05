using UnityEngine;

public class PatrolNode : MonoBehaviour
{
    [Min(0f)] public float minWaitTime = 1f;
    [Min(0f)] public float maxWaitTime = 3f;

    public float GetWaitTime() => Random.Range(minWaitTime, maxWaitTime);

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.6f);
        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.5f,
            $"{gameObject.name}  [{minWaitTime:0.0}–{maxWaitTime:0.0}s]"
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.35f);
    }
#endif
}
