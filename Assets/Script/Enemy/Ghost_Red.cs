using UnityEngine;


[CreateAssetMenu(fileName = "Ghost_Red_SO", menuName = "ScriptableObjects/Ghost_Red_SO")]
public class Ghost_Red : EnemyPathManager
{
    public override Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection, 
        Transform selfPosition, Vector3 otherPosition, Enemy_State mode)
    { 
        return PositionConverter.WorldToGridPos(playerPosition.position);
    }
}
