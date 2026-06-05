using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private PatrolNode[] patrolNodes;
    [SerializeField] private bool randomOrder = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.3f;

    private NavMeshAgent _agent;
    private int _currentNodeIndex = 0;
    private float _waitTimer = 0f;
    private bool _isWaiting = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = moveSpeed;
        _agent.stoppingDistance = stoppingDistance;

        if (patrolNodes.Length > 0)
            MoveToNode(0);
    }

    void Update()
    {
        if (patrolNodes.Length == 0) return;

        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                AdvanceNode();
                MoveToNode(_currentNodeIndex);
            }
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            float wait = patrolNodes[_currentNodeIndex].GetWaitTime();
            if (wait > 0f)
            {
                _isWaiting = true;
                _waitTimer = wait;
            }
            else
            {
                AdvanceNode();
                MoveToNode(_currentNodeIndex);
            }
        }
    }

    void MoveToNode(int index)
    {
        _agent.SetDestination(patrolNodes[index].transform.position);
    }

    void AdvanceNode()
    {
        if (randomOrder)
            _currentNodeIndex = Random.Range(0, patrolNodes.Length);
        else
            _currentNodeIndex = (_currentNodeIndex + 1) % patrolNodes.Length;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (patrolNodes == null || patrolNodes.Length < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolNodes.Length; i++)
        {
            PatrolNode a = patrolNodes[i];
            PatrolNode b = patrolNodes[(i + 1) % patrolNodes.Length];
            if (a != null && b != null)
                Gizmos.DrawLine(a.transform.position, b.transform.position);
        }
    }
#endif
}
