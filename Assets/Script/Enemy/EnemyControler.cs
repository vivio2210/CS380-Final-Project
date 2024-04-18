using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControler : MoveableAgent
{
    public Transform player;
    public Transform otherEnemy;

    public PlayerControl playerControl;
    public EnemyPathManager pathManager;

    [SerializeField]
    private Gridpos finalTargetPosition = new Gridpos(0,0);
    private List<Gridpos> paths = new List<Gridpos>();
    [SerializeField]
    private Vector3 currentTargetPosition;

    [SerializeField]
    private int stepBeforeCompute = 10;
    private int stepCount = 0;

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
    }

    private void SetNewTargetPosition()
    {
        finalTargetPosition = pathManager.SelectTargetPostion(player, playerControl.currentFacingDirection, otherEnemy);
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
            }
        }
        else 
        {
            SetNewTargetPosition();
            stepCount = 0;
        }
        base.SetDestination(currentTargetPosition);

    }
}
