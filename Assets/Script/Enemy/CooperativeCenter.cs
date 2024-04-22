using Script;
using System;
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
    public Color enemyRoomColor;

    [SerializeField]
    public Color lastSeenPlayerPointColor;

    private FloorColorController floorColorController;

    [NonSerialized]
    public List<Gridpos> reservedWanderSpaces = new List<Gridpos>();

    [SerializeField]
    public int enemyVisionMode = 0; // 0 - Always, 1 - LOS, 2 - Propagation

    [SerializeField]
    public int floorDebugMode = 0; // 0 - no color , 1 - Room Door, 2 - Propagation


    public void OnEnable()
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
        if (floorDebugMode == 0)
        {
            ClearColorMap();
        }
        if (floorDebugMode == 1)
        {
            ColorDoorRoomMap();
        }
        if (floorDebugMode == 2)
        {
            ColorPropagateMap();
        }
    }

    public void EnemyVisionModeChange(GameSetting.EnemyVisionMode mode)
    {
        enemyVisionMode = (int)mode;
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
    public void EnemyBehaviorChange(GameSetting.EnemyMode mode)
    {
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            enemyAgents[i].ChangeBehaviorMode(mode);
        }
    }
    public void PlayerControlChange(GameSetting.PlayerMode mode)
    {
        if (mode == 0)
        {
            playerControl.useManualControl = true;
        }
        else
        {
            playerControl.useManualControl = false;
        }
        
    }
    public void PlayerDeadModeChange(GameSetting.EnemyCaptureMode mode)
    {
        if (mode == 0)
        {
            playerControl.hitboxChecker.immortal = false;
            playerControl.surroundMode = false;
        }
        else 
        {
            playerControl.hitboxChecker.immortal = true;
            playerControl.surroundMode = true;
        }
    }

    public void PathDebugChange(GameSetting.EnemyPathDebug mode)
    {
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            if (mode == 0)
            {
                enemyAgents[i].SetDrawPath(false);
            }
            else
            {
                enemyAgents[i].SetDrawPath(true);
            }
        }
    }

    public void FloorDebugChange(GameSetting.FloorDebug mode)
    {
        floorDebugMode = (int)mode;
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
    public void ColorDoorRoomMap()
    {
        floorColorController.ClearColor();
        for (int k = 0; k < enemyAgents.Length; k++)
        {
            Gridpos enemyAgentsPosition = PositionConverter.WorldToGridPos(enemyAgents[k].gameObject.transform.position);
            List<Gridpos> room = AStarPather.GetRoomGrid(enemyAgentsPosition);
            for (int i = 0; i < room.Count; i++)
            {
                floorColorController.ChangeFloorColor(room[i].posx, room[i].posz, enemyRoomColor);
            }
        }
        Gridpos playerAgentsPosition = PositionConverter.WorldToGridPos(playerControl.gameObject.transform.position);
        List<Gridpos> playerRoom = AStarPather.GetRoomGrid(playerAgentsPosition);
        for (int i = 0; i < playerRoom.Count; i++)
        {
            floorColorController.ChangeFloorColor(playerRoom[i].posx, playerRoom[i].posz, roomColor);
        }
        for (int i = 0; i < AStarPather.branchTrackLists.Count; i++)
        {
            Gridpos temp = AStarPather.branchTrackLists[i];
            floorColorController.ChangeFloorColor(temp.posx, temp.posz, doorColor);
        }
    }
    public void ColorPropagateMap()
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
