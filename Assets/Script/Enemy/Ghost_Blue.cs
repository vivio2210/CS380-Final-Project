using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost_Blue : MonoBehaviour
{
    public Transform Player;
    public Transform RedGhost;
    public float UpdateRate = 0.1f;
    private NavMeshAgent Agent;

    private Vector3 currentTargetPosition;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(FollowTarget());
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        while (enabled)
        {
            SetTargetPosition();
            Agent.SetDestination(currentTargetPosition);
            yield return Wait;
        }
    }

    private void SetTargetPosition()
    {
        Vector3 Direction = Player.position + (2 * Player.forward.normalized) - RedGhost.position;
        currentTargetPosition = PositionConverter.WorldToWorld(Player.position + (2 * Direction.normalized));
    }
}
