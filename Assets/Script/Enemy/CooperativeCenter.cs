using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ContentSizeFitter;

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

    [SerializeField]
    private Transform player;

    [SerializeField]
    public EnemyControler[] enemyAgents;

    [SerializeField]
    public bool[] enemiesAssigned;

    [SerializeField]
    public Color enemyColor;

    [SerializeField]
    public Color playerColor;

    //public void SetPlayerPosition(Gridpos playerPos)
    //{
    //    lastSeenPlayerPosition = playerPos;
    //    for (int i = 0; i < enemyAgents.Length; i++)
    //    {
    //        enemyAgents[i].state = Enemy_State.ES_CHASE;
    //    }
    //}

    public void Start()
    {
        enemyAgents = FindObjectsOfType<EnemyControler>();
        enemiesAssigned = new bool[enemyAgents.Length];
        ClearEnemiesAssigned();
        AStarPather.Test();
    }

    public void Update()
    {
        //FloorColorController.GetInstance.ClearColor();
        //AStarPather.Test();
        //List<Gridpos> doors = AStarPather.GetDoorRoomGrid(PositionConverter.WorldToGridPos(player.position));
        //Gridpos temp;
        //for (int i = 0; i < rooms.Count; i++)
        //{
        //    temp = rooms[i];
        //    FloorColorController.GetInstance.ChangeFloorColor(temp.posx, temp.posz, playerColor);
        //}
    }

    public void SetLastSeenPlayerPosition(Gridpos pos)
    {
        lastSeenPlayerPosition = pos;
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            //enemyAgents[i].SetMode(Enemy_State.ES_WANDER);
        }
    }

    public void AssignTasks()
    {
        List<Gridpos> doors = AStarPather.GetDoorRoomGrid(PositionConverter.WorldToGridPos(player.position));

        for (int i = 0; i < enemyAgents.Length; i++)
        {
            if(enemiesAssigned[i] == true)
            { 
                continue; 
            }

        }
    }

    public void AssignTaskChase()
    {
        List<Gridpos> doors = AStarPather.GetDoorRoomGrid(PositionConverter.WorldToGridPos(player.position));

        for (int i = 0; i < enemyAgents.Length; i++)
        {
            if (enemiesAssigned[i] == true)
            {
                continue;
            }



        }
    }

    public void AssignTaskCorner(int index, Gridpos position)
    {
        enemyAgents[index].SetMode(Enemy_State.ES_CORNER);
        enemyAgents[index].otherPosition = PositionConverter.GridPosToWorld(position);
    }

    public void AssignTaskWander(int index)
    {
        enemyAgents[index].SetMode(Enemy_State.ES_CHASE);
    }

    public void ClearEnemiesAssigned()
    {
        for (int i = 0; i < enemiesAssigned.Length; i++)
        {
            enemiesAssigned[i] = false;
        }
    }

}
