using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Ghost_Red : MonoBehaviour
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
        SetTargetPosition();

        while (enabled)
        {
            //if (math.abs(gameObject.transform.position.x - currentTargetPosition.x) <= 1.0f && math.abs(gameObject.transform.position.z - currentTargetPosition.z) <= 1.0f)
            //{
            //    SetTargetPosition();
            //    Debug.Log("Get new target Location : " + currentTargetPosition);
            //}
            SetTargetPosition();
            Agent.SetDestination(PositionConverter.WorldToWorld(currentTargetPosition));
            yield return Wait;
        }
    }

    private void SetTargetPosition()
    {
        currentTargetPosition = Player.transform.position;
    }
}
