using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "Ghost_Cooperative_SO", menuName = "ScriptableObjects/Ghost_Cooperative_SO")]
public class Ghost_Cooperative : EnemyPathManager
{
    [SerializeField]
    private int wanderRadius = 2;

    private Enemy_State mode = Enemy_State.ES_WANDER;
    public override Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection, Transform selfPosition = null, Transform otherPosition = null)
    {
        //if (AStarPather.IsClearPath(PositionConverter.WorldToGridPos(selfPosition.transform.position), PositionConverter.WorldToGridPos(playerPosition.transform.position)))
        //{
        //    return PositionConverter.WorldToGridPos(playerPosition.position);
        //}
        //else
        //{
        //    // Wander
        //    Gridpos Target;
        //    int x = UnityEngine.Random.Range(-wanderRadius, wanderRadius + 1);
        //    int z = UnityEngine.Random.Range(-wanderRadius, wanderRadius + 1);

        //    Target = PositionConverter.WorldToGridPos(selfPosition.position);
        //    Target.posx += x;
        //    Target.posz += z;
        //    return Target;
        //}

        if (mode == Enemy_State.ES_WANDER)
        {
            return Wander(playerPosition, selfPosition);
        }
        else if (mode == Enemy_State.ES_CHASE)
        {
            return Chase(playerPosition, selfPosition);
        }
        else if (mode == Enemy_State.ES_CORNER)
        {
            return Corner(playerPosition, selfPosition);
        }
        else
        {
            return Wander(playerPosition, selfPosition);
        }

    }

    public override void SetMode(Enemy_State state)
    {
        mode = state;
    }

    private Gridpos Wander(Transform playerPosition, Transform selfPosition)
    {
        return PositionConverter.WorldToGridPos(selfPosition.position);
    }
    private Gridpos Chase(Transform playerPosition, Transform selfPosition)
    {
        return PositionConverter.WorldToGridPos(playerPosition.position);
    }
    private Gridpos Corner(Transform playerPosition, Transform selfPosition)
    {
        Gridpos target = AStarPather.GetNearestBranchTrack(PositionConverter.WorldToGridPos(playerPosition.position));
        return target;
    }
}