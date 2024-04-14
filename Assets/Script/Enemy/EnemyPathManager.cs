using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPathManager : ScriptableObject
{
    public virtual Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection, Transform otherPosition = null)
    {
        Debug.Log("Error Abstract Class not overwritten!");
        return new Gridpos(-1,-1);
    }
}
