using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

public class EnemyControler : MoveableAgent
{
    public Transform player;
    public Transform target = null;
    public Vector3 otherPosition;

    public PlayerControl playerControl;
    public EnemyPathManager pathManager;

    [SerializeField]
    private float targetTimer = 3.0f;

    [SerializeField]
    private Gridpos finalTargetPosition = new Gridpos(0, 0);
    private List<Gridpos> paths = new List<Gridpos>();

    [NonSerialized]
    public List<Gridpos> reservedPaths = new List<Gridpos>();

    [SerializeField]
    private Vector3 currentTargetPosition;

    [SerializeField]
    private int stepBeforeCompute = 10;
    [SerializeField]
    private int stepCount = 0;

    [Header("Cooperative Setting")]
    [SerializeField]
    public bool useCooperative = false;
    [SerializeField]
    public Enemy_State currentState = Enemy_State.ES_WANDER;
    [SerializeField]
    public UnityEngine.Color wanderColor = UnityEngine.Color.white;
    [SerializeField]
    public UnityEngine.Color chaseColor = UnityEngine.Color.white;
    [SerializeField]
    public UnityEngine.Color CornerColor = UnityEngine.Color.white;

    private LineRenderer lineRenderer;

    private int firstStep = 0;

    private void Awake()
    {
        AStarPather.initialize();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        SetNewTargetPosition();
        if (useCooperative)
        {
            SetMode(Enemy_State.ES_WANDER);
        }
        //StartCoroutine(TargetTimer());
    }

    private void Update()
    {
        MoveToNextPosition();
        DrawPathLine();
        //if (useCooperative)
        //{
        //    if (currentState != Enemy_State.ES_WANDER)
        //    {
        //        lineRenderer.enabled = true;
        //        DrawPathLine();
        //    }
        //    else
        //    {
        //        lineRenderer.enabled = false;
        //    }
        //}
    }

    public void CustomUpdate()
    {
        MoveToNextPosition();
        if (useCooperative)
        {
            if (currentState != Enemy_State.ES_WANDER)
            {
                lineRenderer.enabled = true;
                DrawPathLine();
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    private void DrawPathLine()
    {
        lineRenderer.positionCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            lineRenderer.SetPosition(i, PositionConverter.GridPosToWorld(paths[i]));
        }
    }
    private void SetNewTargetPosition()
    {
        if (firstStep >= 2 || paths.Count == 0)
        {
            ChooseFinalPosition();

            //paths = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), finalTargetPosition);
            paths = AStarPather.compute_path_with_reserve(PositionConverter.WorldToGridPos(gameObject.transform.position),
                finalTargetPosition, reservedPaths);
            // if imposible not move
            if (paths.Count > 0)
            {
                currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
                paths.Add(paths[paths.Count - 1]);
                paths.RemoveAt(0);
                firstStep = 0;
            }
        }

    }

    private void ChooseFinalPosition()
    {
        if (currentState != Enemy_State.ES_WANDER)
        {
            finalTargetPosition = pathManager.SelectTargetPostion(player, playerControl.currentFacingDirection,
                    gameObject.transform, otherPosition, currentState);
        }
        else
        {
            CooperativeCenter center = FindObjectOfType<CooperativeCenter>();
            finalTargetPosition = center.NearestWander(PositionConverter.WorldToGridPos(gameObject.transform.position));
            if (finalTargetPosition.posx == -1 && finalTargetPosition.posz == -1)
            {
                finalTargetPosition = pathManager.SelectTargetPostion(player, playerControl.currentFacingDirection,
                    gameObject.transform, otherPosition, currentState);
            }
        }
    }

    private void MoveToNextPosition()
    {
        //if (stepCount >= stepBeforeCompute)
        //{
        //    SetNewTargetPosition();
        //    stepCount = 0;
        //}

        if (paths.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
                paths.RemoveAt(0);
                stepCount++;

                if (AStarPather.IsClearPath(PositionConverter.WorldToGridPos(gameObject.transform.position),
                    PositionConverter.WorldToGridPos(player.position)))
                {
                    CooperativeCenter center = FindObjectOfType<CooperativeCenter>();
                    center.SetLastSeenPlayerPosition(PositionConverter.WorldToGridPos(player.position));
                }
                if (useCooperative)
                {
                    CooperativeCenter center = FindObjectOfType<CooperativeCenter>();
                    Gridpos tempPosition = PositionConverter.WorldToGridPos(gameObject.transform.position);
                    if (currentState == Enemy_State.ES_CHASE)
                    {
                        if (center.lastSeenPlayerPosition.posx == tempPosition.posx && center.lastSeenPlayerPosition.posz == tempPosition.posz)
                        {
                            center.ResetTasks();
                        }
                    }
                    else if (currentState == Enemy_State.ES_WANDER)
                    {
                        if(center.ContainInReservedWanderSpaces(tempPosition))
                        {
                            center.RemoveReservedWanderSpaces(tempPosition);
                        }    
                    }
                }

                firstStep++;
            }
        }
        else 
        {
            SetNewTargetPosition();
        }
        base.SetDestination(currentTargetPosition);
    }

    public void SetMode(Enemy_State state)
    {
        if (useCooperative)
        {
            currentState = state;
            SetNewTargetPosition();
            Renderer renderer = GetComponent<Renderer>();
            if (currentState == Enemy_State.ES_WANDER)
            {
                renderer.material.color = wanderColor;    
            }
            else if (currentState == Enemy_State.ES_CHASE)
            {
                renderer.material.color = chaseColor;
            }
            else if (currentState == Enemy_State.ES_CORNER)
            {
                renderer.material.color = CornerColor;
            }
        }
    }

    private IEnumerator TargetTimer()
    {
        WaitForSeconds Wait = new WaitForSeconds(targetTimer);

        while (enabled)
        {
            Debug.Log("Timer");
            SetNewTargetPosition();
            yield return Wait;
        }
    }

    public void ClearReserved()
    {
        reservedPaths.Clear();
    }

}
