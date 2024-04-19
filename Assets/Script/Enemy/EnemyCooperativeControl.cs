using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCooperativeControl : MoveableAgent
{
    public Transform playerTransform;

    [SerializeField]
    private int wanderRadius = 3;

    private Gridpos finalTargetPosition = new Gridpos(0, 0);
    private List<Gridpos> paths = new List<Gridpos>();
    private Vector3 currentTargetPosition;

    [SerializeField]
    public Enemy_State state = Enemy_State.ES_WANDER;

    private void Awake()
    {
        AStarPather.initialize();
    }

    private void Update()
    {
        if (state == Enemy_State.ES_WANDER)
        {
            Wander();
        }
        else if(state == Enemy_State.ES_CHASE)
        {
            Chase();
        }
        else if (state == Enemy_State.ES_CORNER)
        {
            Corner();
        }
    }

    private void Wander()
    {
        if (paths.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f &&
                math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
                paths.RemoveAt(0);
            }
            base.SetDestination(currentTargetPosition);
        }
        else
        {
            finalTargetPosition = RandomDestination();
            Debug.Log(finalTargetPosition.posx);
            Debug.Log(finalTargetPosition.posz);
            paths = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), finalTargetPosition);
            
            if (AStarPather.IsClearPath(PositionConverter.WorldToGridPos(gameObject.transform.position), 
                PositionConverter.WorldToGridPos(playerTransform.position)))
            {
                CooperativeCenter center = FindObjectOfType<CooperativeCenter>();
                center.SetLastSeenPlayerPosition(PositionConverter.WorldToGridPos(playerTransform.position));
            }
        }
    }
    private void Chase()
    {
        if (paths.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f &&
                math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
                paths.RemoveAt(0);
            }
            base.SetDestination(currentTargetPosition);
        }
        else
        {
            finalTargetPosition = PositionConverter.WorldToGridPos(playerTransform.position);
            paths = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), finalTargetPosition);

            if (AStarPather.IsClearPath(PositionConverter.WorldToGridPos(gameObject.transform.position),
                PositionConverter.WorldToGridPos(playerTransform.position)))
            {
                CooperativeCenter center = FindObjectOfType<CooperativeCenter>();
                center.SetLastSeenPlayerPosition(PositionConverter.WorldToGridPos(playerTransform.position));
            }
        }
    }
    private void Corner()
    {

    }
    private Gridpos RandomDestination()
    {
        Gridpos selfpos = PositionConverter.WorldToGridPos(gameObject.transform.position);
        while (true) 
        {
            int x = UnityEngine.Random.Range(-wanderRadius, wanderRadius + 1);
            int z = UnityEngine.Random.Range(-wanderRadius, wanderRadius + 1);
            if (!MapChecker.IsWall(selfpos + new Gridpos(x,z)))
            {
                selfpos.posx += x;
                selfpos.posz += z;
                break;
            }
        }
        return selfpos;
    }
}
