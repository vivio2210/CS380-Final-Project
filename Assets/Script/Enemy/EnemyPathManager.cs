using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPathManager : ScriptableObject
{
    public virtual Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection, Transform selfPosition = null, Transform otherPosition = null)
    {
        Debug.LogError("Error Abstract Class not overwritten!");
        return new Gridpos(-1,-1);
    }
    public virtual void SetMode(Enemy_State state)
    {
        Debug.LogError("Error Abstract Class not overwritten!");
    }
}
