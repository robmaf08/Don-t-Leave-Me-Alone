using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Components")]
    private NavMeshAgent _navMeshAgent;
    private Transform CurrentTarget;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float stopDistance = 2.0f;
    [SerializeField] private float waitTime = 2.0f;
    [SerializeField] private bool randomizePoints;
    private float distanceToTarget;
    private int targetNumber = 0;
    private Animator _animator;
    private bool isMoving = false;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        CurrentTarget = targets[0];
    }

    void Update()
    {
        distanceToTarget = Vector3.Distance(CurrentTarget.position, transform.position);
        if (distanceToTarget > stopDistance)
        {
            if (!isMoving)
            {
                _animator.Play("Run");
                isMoving = true;
            }

            _navMeshAgent.SetDestination(CurrentTarget.position);
            _navMeshAgent.isStopped = false;
        }
        if (distanceToTarget < stopDistance)
        {
            _navMeshAgent.isStopped = true;
            _animator.Play("Idle");
            targetNumber++;

            if (targetNumber >= targets.Count)
            {
                targetNumber = 0;
                //targets[0].transform.position = new Vector3(Random.Range(100, 400), 0, Random.Range(100, 400));
            }
            CurrentTarget = targets[targetNumber];
            isMoving = false;
            StartCoroutine(LookAround());
        }
    }

    private IEnumerator LookAround() 
    {
        if (waitTime > 0)
        {
            enabled = false;
            yield return new WaitForSeconds(waitTime);
            enabled = true;
        } else
        {
            yield return null;  
        }
    }

    public void StopFollowingPath() 
    {
        StopAllCoroutines();
        StopCoroutine(LookAround());
        _animator.Rebind();
        enabled = false;
        targetNumber = 0;
    }

    /* Draw a path with lines that enemy will follow (if any)
     * in the scene editor */
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        if (targets != null && targets.Count > 0)
        {
            for (int i = 1; i < targets.Count; i++)
            {
                Gizmos.DrawLine(targets[i-1].position, targets[i].position);
                Gizmos.DrawSphere(targets[i].position, 1);
            }
            Gizmos.DrawLine(targets[0].position, targets[targets.Count - 1].position);
            Gizmos.DrawSphere(targets[0].position, 1);
        }
    }

}