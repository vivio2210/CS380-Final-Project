using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Ghost_Red_SO", menuName = "ScriptableObjects/Ghost_Red_SO")]
public class Ghost_Red : EnemyPathManager
{
    public override Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection, Transform selfPosition, Vector3 otherPosition) 
    { 
        return PositionConverter.WorldToGridPos(playerPosition.position);
    }
}
