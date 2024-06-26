using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPathManager : ScriptableObject
{
    public virtual Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection,
        Transform selfPosition, Vector3 otherPosition, Enemy_State mode)
    {
        Debug.LogError("Error Abstract Class not overwritten!");
        return new Gridpos(-1,-1);
    }
}
