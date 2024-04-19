using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Ghost_Pink_SO", menuName = "ScriptableObjects/Ghost_Pink_SO")]
public class Ghost_Pink : EnemyPathManager
{
    public override Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection, Transform selfPosition , Vector3 otherPosition)
    {
        Vector3 goal = playerPosition.position + 4 * playerFaceDirection;
        return PositionConverter.WorldToGridPos(goal);
    }
}
