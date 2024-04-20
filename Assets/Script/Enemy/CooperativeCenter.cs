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
    public Color doorColor;

    [SerializeField]
    public Color roomColor;

    private FloorColorController floorColorController;

    public void Start()
    {
        floorColorController = FindObjectOfType<FloorColorController>();
        enemyAgents = FindObjectsOfType<EnemyControler>();
        enemiesAssigned = new bool[enemyAgents.Length];
        ClearEnemiesAssigned();
        //AStarPather.Test();
    }

    public void Update()
    {
        floorColorController.ClearColor();
        //AStarPather.Test();
        //List<Gridpos> doors = AStarPather.GetDoorRoomGrid(PositionConverter.WorldToGridPos(player.position));
        List<Gridpos> rooms = AStarPather.GetRoomGrid(PositionConverter.WorldToGridPos(player.position));
        Gridpos temp;

        for (int i = 0; i < AStarPather.branchTrackLists.Count; i++)
        {
            temp = AStarPather.branchTrackLists[i];
            floorColorController.ChangeFloorColor(temp.posx, temp.posz, doorColor);
        }
        for (int i = 0; i < rooms.Count; i++)
        {
            temp = rooms[i];
            floorColorController.ChangeFloorColor(temp.posx, temp.posz, roomColor);
        }
    }

    public void SetLastSeenPlayerPosition(Gridpos pos)
    {
        lastSeenPlayerPosition = pos;
        AssignTasks();
    }

    public void AssignTasks()
    {
        ClearEnemiesAssigned();
        int index = -1;
        float currentdistance = 5000;
        Gridpos playerPos = PositionConverter.WorldToGridPos(player.transform.position);
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            Gridpos enemyPos = PositionConverter.WorldToGridPos(enemyAgents[i].gameObject.transform.position);
            float distance = AStarPather.FindActualDistance(enemyPos.posx, enemyPos.posz, playerPos.posx, playerPos.posz);
            if (distance < currentdistance)
            {
                currentdistance = distance;
                index = i;
            }
        }
        enemyAgents[index].player.position = PositionConverter.GridPosToWorld(lastSeenPlayerPosition);
        enemyAgents[index].SetMode(Enemy_State.ES_CHASE);
        enemiesAssigned[index] = true;
        List<Gridpos> doors = AStarPather.GetDoorRoomGrid(PositionConverter.WorldToGridPos(player.position));
        List<Gridpos> rooms = AStarPather.GetRoomGrid(PositionConverter.WorldToGridPos(player.position));
        for (int i = 0; i < doors.Count; i++)
        {
            index = -1;
            currentdistance = 5000;
            playerPos = doors[i];
            for (int j = 0; j < enemyAgents.Length; j++)
            {
                if (enemiesAssigned[j] == true)
                {
                    continue;
                }
                Gridpos enemyPos = PositionConverter.WorldToGridPos(enemyAgents[j].gameObject.transform.position);
                float distance = AStarPather.FindActualDistance(enemyPos.posx, enemyPos.posz, playerPos.posx, playerPos.posz);
                if (distance < currentdistance)
                {
                    currentdistance = distance;
                    index = j;
                }
            }
            if (index != -1)
            {
                enemyAgents[index].otherPosition = PositionConverter.GridPosToWorld(doors[i]);
                enemyAgents[index].reservedPaths = rooms;
                enemyAgents[index].SetMode(Enemy_State.ES_CORNER);
                enemiesAssigned[index] = true;
                //rooms.Add(doors[i]);
            }
        }
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            if (enemiesAssigned[i] == true)
            {
                continue;
            }
            enemyAgents[i].SetMode(Enemy_State.ES_WANDER);
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
