using UnityEngine.AI;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Camera Camera = null;
    private NavMeshAgent Agent;

    private RaycastHit[] Hits = new RaycastHit[1];

    [SerializeField]
    private GameObject[] enemies = null;

    private int mapSize = 20;
    private int enemyDetectingArea = 25; // should be odd number

    private int pathCount = 0;

    [SerializeField]
    private Gridpos currentTargetPosition = new Gridpos(0, 0);
    private List<Gridpos> currentPath = new List<Gridpos>();

    [SerializeField]
    private Vector3 currentTargetPosition_Vec3 = new Vector3(0, 0, 0);

    [SerializeField]
    private Gridpos currentGridPosition = new Gridpos(0, 0);

    [SerializeField]
    public Vector3 currentFacingDirection = new Vector3(0, 0, 1);

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        AStarPather.initialize();
    }

    private void Update()
    {
        MoveToFarthestDistance();
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
            if (currentPath.Count > 0 && pathCount < 5)
            {
                if (math.abs(currentTargetPosition_Vec3.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition_Vec3.z - gameObject.transform.position.z) <= 0.1f)
                {
                    currentGridPosition = currentPath[0];
                    currentPath.RemoveAt(0);
                    currentTargetPosition_Vec3 = PositionConverter.GridPosToWorld(currentGridPosition);
                    pathCount++;
                }
                Agent.SetDestination(currentTargetPosition_Vec3);
                return;
            }

            AStarPather.ComputeNeighbor();

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

            currentPath = AStarPather.compute_path(playerGrisPos, farthestGrid);
            currentGridPosition = new Gridpos(currentPath[0].posx, currentPath[0].posz);
            currentPath.RemoveAt(0);

            if(currentPath.Count > 0)
            {
                currentPath.Add(currentPath[currentPath.Count - 1]);
            }

            currentTargetPosition_Vec3 = PositionConverter.GridPosToWorld(currentGridPosition);
            Agent.SetDestination(currentTargetPosition_Vec3);

            pathCount = 0;
        }
    }

    private void SetFaceDirection()
    {
        currentGridPosition = PositionConverter.WorldToGridPos(gameObject.transform.position);
        currentFacingDirection = new Vector3(currentTargetPosition_Vec3.x - currentGridPosition.posx, 0, currentTargetPosition_Vec3.z - currentGridPosition.posz).normalized;
    }
}