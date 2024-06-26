using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Ghost_Orange_SO", menuName = "ScriptableObjects/Ghost_Orange_SO")]
public class Ghost_Orange : EnemyPathManager
{
    public override Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection,
        Transform selfPosition, Vector3 otherPosition, Enemy_State mode)
    {
        float distance = math.sqrt(math.pow(otherPosition.x - playerPosition.position.x,2) + math.pow(otherPosition.z - playerPosition.position.z, 2));
        if (distance > 8.0f)
        {
            return PositionConverter.WorldToGridPos(playerPosition.position);
        }
        else
        {
            return new Gridpos(18, 19);
        }
    }
}
