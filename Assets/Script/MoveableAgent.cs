using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableAgent : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    private Vector3 currentTargetDirection = new Vector3(0, 0, 0);
    protected void SetDestination(Vector3 targetPosition)
    {
        currentTargetDirection = targetPosition - transform.position;
        transform.position += speed * Time.deltaTime * currentTargetDirection.normalized;
    }
}
