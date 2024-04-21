using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Script.GameSetting;
using static UnityEditor.PlayerSettings;
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
    public Gridpos lastSeenPlayerPosition = new Gridpos(-1, -1);

    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerControl playerControl;

    [SerializeField]
    public EnemyControler[] enemyAgents;

    [SerializeField]
    public bool[] enemiesAssigned;

    [SerializeField]
    public Color doorColor;

    [SerializeField]
    public Color roomColor;

    [SerializeField]
    public Color lastSeenPlayerPointColor;

    private FloorColorController floorColorController;

    [NonSerialized]
    public List<Gridpos> reservedWanderSpaces = new List<Gridpos>();

    [SerializeField]
    public int enemyVisionMode = 0; // 0 - Always, 1 - LOS, 2 - Propagation

    public void Start()
    {
        floorColorController = FindObjectOfType<FloorColorController>();
        enemyAgents = FindObjectsOfType<EnemyControler>();
        enemiesAssigned = new bool[enemyAgents.Length];
        ClearEnemiesAssigned();
        //EnemyPropagationMap.initialize();
        //EnemyPropagationMap.layer[0, 0] = 1;
        //EnemyPropagationMap.layer[9, 9] = -1;
        //AStarPather.Test();
    }

    public void Update()
    {
        if (enemyVisionMode == 1)
        {
            EnemyVision();
        }
        if (enemyVisionMode == 2)
        {
            EnemyVision();
            Propagation();
        }
        ColorMap();
    }

    public void EnemyVisionModeChange()
    {
        enemyVisionMode++;
        enemyVisionMode = enemyVisionMode % 3;
        if (enemyVisionMode == 0)
        {

        }
        else if (enemyVisionMode == 1)
        {

        }
        else
        {
            EnemyPropagationMap.initialize();
        }
    }
    public void EnemyBehaviorChange()
    {
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            enemyAgents[i].ChangeBehaviorMode();
        }
    }
    public void PlayerControlChange()
    {
        playerControl.useManualControl = !playerControl.useManualControl;
    }
    public void PlayerDeadModeChange()
    {
        playerControl.hitboxChecker.immortal = !playerControl.hitboxChecker.immortal;
        playerControl.surroundMode = !playerControl.surroundMode;
    }

    public void EnemyVision()
    {
        List<Gridpos> enemySeenPaths = new List<Gridpos>();

        for (int k = 0; k < enemyAgents.Length; k++)
        {
            Gridpos enemyAgentsPosition = PositionConverter.WorldToGridPos(enemyAgents[k].gameObject.transform.position);
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (EnemyPropagationMap.layer[i, j] < 0.0f)
                    {
                        EnemyPropagationMap.layer[i, j] = 0.0f;
                    }
                    if (AStarPather.IsClearPath(enemyAgentsPosition, new Gridpos(i, j)))
                    {
                        enemySeenPaths.Add(new Gridpos(i, j));
                    }
                }
            }
        }
        for (int i = 0; i < enemySeenPaths.Count; i++)
        {
            EnemyPropagationMap.layer[enemySeenPaths[i].posx, enemySeenPaths[i].posz] = -1;
        }
    }
    public void Propagation()
    {
        EnemyPropagationMap.Propagate(Time.deltaTime);
        EnemyPropagationMap.NormalizePropagate(Time.deltaTime);
    }
    public void ClearColorMap()
    {
        floorColorController.ClearColor();
    }
    public void ColorMap()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (EnemyPropagationMap.layer[i, j] < 0.0f)
                {
                    floorColorController.ChangeFloorColor(i, j, new Color(0.5f, 0.5f, 1.0f, 1.0f));
                }
                else
                {
                    floorColorController.ChangeFloorColor(i, j, new Color(1, 1.0f - EnemyPropagationMap.layer[i, j],
                        1.0f - EnemyPropagationMap.layer[i, j], 1.0f));
                }

            }
        }
    }

    public Gridpos NearestWander(Gridpos pos)
    {
        float highestValue = -10.0f;
        int indexI = -1;
        int indexJ = -1;
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (!ContainInReservedWanderSpaces(new Gridpos(i,j)))
                {
                    float distance = AStarPather.FindDistance(pos.posx, pos.posz, i, j);
                    distance = 400000 - distance;
                    float value = EnemyPropagationMap.layer[i, j] * distance;
                    if (value > highestValue)
                    {
                        highestValue = value;
                        indexI = i;
                        indexJ = j;
                    }
                    
                }
            }
        }
        if (highestValue <= 0.1f)
        {
            return new Gridpos(-1, -1);
        }
        else 
        {
            reservedWanderSpaces.Add(new Gridpos(indexI, indexJ));
        }
        return new Gridpos(indexI, indexJ);
    }

    public bool ContainInReservedWanderSpaces(Gridpos pos) 
    {
        bool founded = false;
        for (int i = 0; i < reservedWanderSpaces.Count; i++)
        {
            float distance = AStarPather.FindDistance(pos.posx, pos.posz, reservedWanderSpaces[i].posx, reservedWanderSpaces[i].posz);
            //if (reservedWanderSpaces[i].posx == pos.posx && reservedWanderSpaces[i].posz == pos.posz)
            if (distance <= 4.0f)
            {
                founded = true;
            }
        }
        return founded; 
    }
    public void RemoveReservedWanderSpaces(Gridpos reservedPos)
    {
        for (int i = 0; i < reservedWanderSpaces.Count; i++)
        {
            if (reservedWanderSpaces[i].posx == reservedPos.posx && reservedWanderSpaces[i].posz == reservedPos.posz)
            {
                reservedWanderSpaces.RemoveAt(i);
                break;
            }
        }
    }

    public void SetLastSeenPlayerPosition(Gridpos pos)
    {
        lastSeenPlayerPosition = pos;
        if (enemyVisionMode >= 1)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (pos.posx == i && pos.posz == j)
                    {
                        EnemyPropagationMap.layer[i, j] = 1.0f;
                    }
                    else
                    {
                        EnemyPropagationMap.layer[i, j] = 0.0f;
                    }
                }
            }
        }
        EnemyPropagationMap.Propagate(Time.deltaTime);
        EnemyPropagationMap.NormalizePropagate(Time.deltaTime);
        AssignTasks();
    }

    public void ResetTasks()
    {
        ClearEnemiesAssigned();
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            enemyAgents[i].ClearReserved();
            enemyAgents[i].SetMode(Enemy_State.ES_WANDER);
        }
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
        enemyAgents[index].otherPosition = PositionConverter.GridPosToWorld(lastSeenPlayerPosition);
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
            enemyAgents[i].ClearReserved();
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
        enemyAgents[index].SetMode(Enemy_State.ES_WANDER);
    }

    public void ClearEnemiesAssigned()
    {
        for (int i = 0; i < enemiesAssigned.Length; i++)
        {
            enemiesAssigned[i] = false;
        }
    }

}
