using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControler : MonoBehaviour
{
    public Transform player;
    public Transform otherEnemy;

    public PlayerControl playerControl;
    public EnemyPathManager pathManager;

    private NavMeshAgent Agent;
    private Gridpos finalTargetPosition = new Gridpos(0,0);
    private List<Gridpos> paths = new List<Gridpos>();
    private Vector3 currentTargetPosition;

    [SerializeField]
    private int stepBeforeCompute = 10;
    [SerializeField]
    private int stepCount = 0;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        AStarPather.initialize();
    }

    private void Start()
    {
        SetNewTargetPosition();
    }

    //private IEnumerator FollowTarget()
    //{
    //    WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
    //    while (enabled)
    //    {
    //        MoveToNextPosition();
    //        yield return Wait;
    //    }
    //}

    private void Update()
    {
        MoveToNextPosition();
    }

    private void SetNewTargetPosition()
    {
        finalTargetPosition = pathManager.SelectTargetPostion(player, playerControl.currentFacingDirection, otherEnemy);
        paths = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), finalTargetPosition);

        currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
        paths.RemoveAt(0);
        Agent.SetDestination(currentTargetPosition);
    }

    private void MoveToNextPosition()
    {
        if (stepCount >= stepBeforeCompute)
        {
            SetNewTargetPosition();
            stepCount = 0;
        }

        if (paths.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
                paths.RemoveAt(0);
                stepCount++;
            }
            Agent.SetDestination(currentTargetPosition);
        }
        else 
        {
            SetNewTargetPosition();
            stepCount = 0;
        }
    }
}
