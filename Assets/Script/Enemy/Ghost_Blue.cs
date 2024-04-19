using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Ghost_Blue_SO", menuName = "ScriptableObjects/Ghost_Blue_SO")]
public class Ghost_Blue : EnemyPathManager
{
    public override Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection,
        Transform selfPosition, Vector3 otherPosition, Enemy_State mode)
    {
        Vector3 playerLookAtPostion = playerPosition.position + 2 * playerFaceDirection;
        Vector3 direction = playerLookAtPostion - otherPosition;
        Vector3 goal = otherPosition + 4 * direction.normalized;
        return PositionConverter.WorldToGridPos(goal);
    }
}
