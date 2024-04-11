using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Ghost_Orange : MonoBehaviour
{
    public Transform Player;
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
        float distance = math.sqrt(math.pow(gameObject.transform.position.x - Player.position.x,2) + math.pow(gameObject.transform.position.z - Player.position.z,2));
        if (distance > 8)
        {
            currentTargetPosition = PositionConverter.WorldToWorld(Player.position);
        }
        else
        {
            currentTargetPosition = new Vector3(18.5f, 1, 19.5f);
        }
    }
}
