using UnityEngine.AI;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

public class PlayerControl : MoveableAgent
{
    [SerializeField]
    private bool useManualControl = false;
    [SerializeField]
    private Camera Camera = null;
    //private NavMeshAgent Agent;

    private RaycastHit[] Hits = new RaycastHit[1];

    [SerializeField]
    private GameObject[] enemies = null;

    private int mapSize = 20;
    private int enemyDetectingArea = 25; // should be odd number

    private int pathCount = 0;

    [SerializeField]
    private int stepBeforeCompute = 1;
    private List<Gridpos> currentPath = new List<Gridpos>();

    [SerializeField]
    private Vector3 currentTargetPosition_Vec3 = new Vector3(0, 0, 0);

    [SerializeField]
    private Gridpos currentGridPosition = new Gridpos(0, 0);

    [SerializeField]
    public Vector3 currentFacingDirection = new Vector3(0, 0, 1);


    private Vector3 currentTargetPosition = new Vector3(0, 0, 0);
    private Gridpos currentTargetGridPosition = new Gridpos(0, 0);

    private void Awake()
    {
        //Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        AStarPather.initialize();
        updateEnemyInScene();
        if (useManualControl)
        {
            currentTargetGridPosition = PositionConverter.WorldToGridPos(gameObject.transform.position);
            currentTargetPosition = PositionConverter.GridPosToWorld(currentTargetGridPosition);
        }
    }

    private void Update()
    {
        if (useManualControl)
        {
            ManualControl();
        }
        else
        {
            MoveToFarthestDistance();
        }
        SetFaceDirection();
    }

    private void MoveToFarthestDistance()
    {
        float maxDistance = 0;
        float radius = Mathf.Floor(enemyDetectingArea / 2);

        Gridpos farthestGrid = new Gridpos(0, 0);
        Gridpos playerGrisPos = new Gridpos(PositionConverter.WorldToGridPos(this.gameObject.transform.position).posx,
            PositionConverter.WorldToGridPos(this.gameObject.transform.position).posz);

        if (enemies != null)
        {
            // if have path
            if (currentPath.Count > 0 && pathCount < stepBeforeCompute)
            {
                if (math.abs(currentTargetPosition_Vec3.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition_Vec3.z - gameObject.transform.position.z) <= 0.1f)
                {
                    currentGridPosition = currentPath[0];
                    currentPath.RemoveAt(0);
                    currentTargetPosition_Vec3 = PositionConverter.GridPosToWorld(currentGridPosition);
                    pathCount++;
                }
                base.SetDestination(currentTargetPosition_Vec3);
                return;
            }

            AStarPather.PlayerComputeNeighbor();

            for (int row = -(int)radius; row < radius + 1; row++)
            {
                for (int col = -(int)radius; col < radius + 1; col++)
                {
                    Gridpos currentGrid = new Gridpos(playerGrisPos.posx + col, playerGrisPos.posz + row);
                    if ((currentGrid.posx >= 0 || currentGrid.posx < mapSize) && (currentGrid.posz >= 0 || currentGrid.posz < mapSize)
                        && MapChecker.IsWall(currentGrid) == false)
                    {
                        float distance = 0;
                        float totalDistance = 0;

                        for (int i = 0; i < enemies.Length; i++)
                        {
                            // find distance from gridPos to all enemies and check that is wall or not
                            Gridpos enemyPosition = PositionConverter.WorldToGridPos(enemies[i].gameObject.transform.position);

                            distance = Mathf.Pow(enemyPosition.posx - currentGrid.posx, 2) + Mathf.Pow(enemyPosition.posz - currentGrid.posz, 2);
                            if (distance <= 1.0f)
                                distance *= 0;
                            else if (distance <= 3.0f)
                                distance *= 0.2f;
                            else if (distance <= 5.0f)
                                distance *= 0.4f;
                            else if (distance <= 7.0f)
                                distance *= 0.6f;
                            else if (distance <= 9.0f)
                                distance *= 0.8f;

                            totalDistance += distance;
                        }

                        if (totalDistance > maxDistance)
                        {
                            maxDistance = totalDistance;
                            farthestGrid.posx = currentGrid.posx;
                            farthestGrid.posz = currentGrid.posz;
                        }
                    }
                }
            }

            //currentTargetPosition = farthestGrid;
            //Agent.SetDestination(PositionConverter.GridPosToWorld(farthestGrid));

            currentPath.Clear();

            currentPath = AStarPather.compute_path(playerGrisPos, farthestGrid, true);
            if (currentPath.Count > 0)
            {
                currentGridPosition = new Gridpos(currentPath[0].posx, currentPath[0].posz);
                currentPath.Add(currentPath[currentPath.Count - 1]);
                currentPath.RemoveAt(0);
            }

            currentTargetPosition_Vec3 = PositionConverter.GridPosToWorld(currentGridPosition);
            base.SetDestination(currentTargetPosition_Vec3);

            pathCount = 0;
        }
    }

    private void SetFaceDirection()
    {
        Gridpos playerPositon = PositionConverter.WorldToGridPos(gameObject.transform.position);
        Gridpos goalPositon = PositionConverter.WorldToGridPos(currentTargetPosition_Vec3);

        currentFacingDirection = new Vector3(playerPositon.posx - goalPositon.posx, 0, playerPositon.posz - goalPositon.posz).normalized;
    }
    public void updateEnemyInScene() // Use this function every time you increase or decrease the number of enemies.
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public int GetPlayerStep()
    {
        return stepBeforeCompute;
    }

    public void SetPlayerStep(int playerStep)
    {
        stepBeforeCompute = playerStep;
    }

    private void ManualControl()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, Hits) > 0)
            {
                //AStarPather.PlayerComputeNeighbor();
                currentPath = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), 
                    PositionConverter.WorldToGridPos(Hits[0].point), true);
                
                if (currentPath.Count > 0)
                {
                    currentTargetGridPosition = new Gridpos(currentPath[0].posx, currentPath[0].posz);
                    currentTargetPosition = PositionConverter.GridPosToWorld(currentTargetGridPosition);
                    currentPath.RemoveAt(0);
                }
            }
        }

        if (currentPath.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentTargetGridPosition = currentPath[0];
                currentTargetPosition = PositionConverter.GridPosToWorld(currentTargetGridPosition);
                currentPath.RemoveAt(0);
            }
            base.SetDestination(currentTargetPosition);
        }
        else
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                base.SetDestination(PositionConverter.WorldToWorld(gameObject.transform.position));
            }
            base.SetDestination(currentTargetPosition);
        }
    }

    private void KeyboardManualControl()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, Hits) > 0)
            {
                currentPath = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), PositionConverter.WorldToGridPos(Hits[0].point));
                if (currentPath.Count > 0)
                {
                    currentTargetGridPosition = new Gridpos(currentPath[0].posx, currentPath[0].posz);
                    currentTargetPosition = PositionConverter.GridPosToWorld(currentTargetGridPosition);
                    currentPath.RemoveAt(0);
                }
            }
        }

        if (currentPath.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentTargetGridPosition = currentPath[0];
                currentTargetPosition = PositionConverter.GridPosToWorld(currentTargetGridPosition);
                currentPath.RemoveAt(0);
            }
            base.SetDestination(currentTargetPosition);
        }
        else
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                base.SetDestination(PositionConverter.WorldToWorld(gameObject.transform.position));
            }
            base.SetDestination(currentTargetPosition);
        }
    }
}