using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControler : MoveableAgent
{
    public Transform player;
    public Transform target = null;
    public Transform otherEnemy;

    public PlayerControl playerControl;
    public EnemyPathManager pathManager;

    private Gridpos finalTargetPosition = new Gridpos(0, 0);
    private List<Gridpos> paths = new List<Gridpos>();
    private Vector3 currentTargetPosition;

    [SerializeField]
    private int stepBeforeCompute = 10;
    private int stepCount = 0;

    [Header("Cooperative Setting")]
    [SerializeField]
    private bool useCooperative = false;

    private void Awake()
    {
        AStarPather.initialize();
    }

    private void Start()
    {
        SetNewTargetPosition();
    }

    private void Update()
    {
        MoveToNextPosition();
        pathManager.SetMode(Enemy_State.ES_CHASE);
    }

    private void SetNewTargetPosition()
    {
        finalTargetPosition = pathManager.SelectTargetPostion(player, playerControl.currentFacingDirection, gameObject.transform, otherEnemy);
        paths = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), finalTargetPosition);

        // if imposible not move
        if (paths.Count > 0)
        {
            currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
            base.SetDestination(currentTargetPosition);
        }
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
                if (paths.Count != 1)
                    paths.RemoveAt(0);
                currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
                stepCount++;

                if (AStarPather.IsClearPath(PositionConverter.WorldToGridPos(gameObject.transform.position),
                    PositionConverter.WorldToGridPos(player.position)))
                {
                    CooperativeCenter center = FindObjectOfType<CooperativeCenter>();
                    center.SetLastSeenPlayerPosition(PositionConverter.WorldToGridPos(player.position));
                }
            }
        }
        else
        {
            SetNewTargetPosition();
            stepCount = 0;
        }
        base.SetDestination(currentTargetPosition);
    }

    //private bool IsSeePlayer()
    //{
    //    if (AStarPather.IsClearPath(PositionConverter.WorldToGridPos(gameObject.transform.position), 
    //        PositionConverter.WorldToGridPos(player.transform.position)))
    //    { 
            
    //    }
    //}

}
