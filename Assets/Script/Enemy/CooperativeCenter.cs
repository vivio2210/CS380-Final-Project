using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy_State
{
    ES_WANDER,
    ES_CHASE,
    ES_CORNER
};

public class CooperativeCenter : MonoBehaviour
{
    [SerializeField]
    private Gridpos lastSeenPlayerPosition = new Gridpos(-1, -1);

    //[SerializeField]
    //public EnemyCooperativeControl[] enemyAgents;

    //public void SetPlayerPosition(Gridpos playerPos)
    //{
    //    lastSeenPlayerPosition = playerPos;
    //    for (int i = 0; i < enemyAgents.Length; i++)
    //    {
    //        enemyAgents[i].state = Enemy_State.ES_CHASE;
    //    }
    //}

    public void SetLastSeenPlayerPosition(Gridpos pos)
    {
        lastSeenPlayerPosition = pos;
    }

}
