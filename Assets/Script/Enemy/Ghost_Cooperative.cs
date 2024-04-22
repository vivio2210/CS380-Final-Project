using UnityEngine;


[CreateAssetMenu(fileName = "Ghost_Cooperative_SO", menuName = "ScriptableObjects/Ghost_Cooperative_SO")]
public class Ghost_Cooperative : EnemyPathManager
{
    [SerializeField]
    private int wanderRadius = 2;

    public override Gridpos SelectTargetPostion(Transform playerPosition, Vector3 playerFaceDirection, 
        Transform selfPosition, Vector3 otherPosition, Enemy_State mode)
    {
        //Debug.Log(mode);
        if (mode == Enemy_State.ES_WANDER)
        {
            return Wander(playerPosition, selfPosition, otherPosition);
        }
        else if (mode == Enemy_State.ES_CHASE)
        {
            return Chase(playerPosition, selfPosition, otherPosition);
        }
        else if (mode == Enemy_State.ES_CORNER)
        {
            return Corner(playerPosition, selfPosition, otherPosition);
        }
        else
        {
            return Wander(playerPosition, selfPosition, otherPosition);
        }

    }

    private Gridpos Wander(Transform playerPosition, Transform selfPosition, Vector3 otherPosition)
    {
        // Wander
        Gridpos Target;
        int x = UnityEngine.Random.Range(-wanderRadius, wanderRadius + 1);
        int z = UnityEngine.Random.Range(-wanderRadius, wanderRadius + 1);

        Target = PositionConverter.WorldToGridPos(selfPosition.position);
        Target.posx += x;
        Target.posz += z;
        return Target;
    }
    private Gridpos Chase(Transform playerPosition, Transform selfPosition, Vector3 otherPosition)
    {
        return PositionConverter.WorldToGridPos(playerPosition.position);
    }
    private Gridpos Corner(Transform playerPosition, Transform selfPosition, Vector3 otherPosition)
    {
        return PositionConverter.WorldToGridPos(otherPosition);
    }
}
