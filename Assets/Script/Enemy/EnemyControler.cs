using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField]
    private Vector3 currentTargetPosition;

    [SerializeField]
    private int stepBeforeCompute = 10;
    [SerializeField]
    private int stepCount = 0;

    [Header("Cooperative Setting")]
    [SerializeField]
    public bool useCooperative = false;

    private void Awake()
    {
        AStarPather.initialize();
    }

    private void Start()
    {
        SetNewTargetPosition();
        if (useCooperative)
        {
            pathManager.SetMode(Enemy_State.ES_CORNER);
        }
        StartCoroutine(TargetTimer());
    }

    private void Update()
    {
        MoveToNextPosition();
    }

    private void SetNewTargetPosition()
    {
        finalTargetPosition = pathManager.SelectTargetPostion(player, playerControl.currentFacingDirection, gameObject.transform, otherPosition);
        paths = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), finalTargetPosition);

        //for (int i = 0;i < paths.Count; i++)
        //    Debug.Log(paths[i].posx + ", " + paths[i].posz);

        // if imposible not move
        if (paths.Count > 0)
        {
            currentTargetPosition = PositionConverter.GridPosToWorld(paths[0]);
            paths.Add(paths[paths.Count - 1]);
            paths.RemoveAt(0);
        }
    }

    private void MoveToNextPosition()
    {
        if (stepCount >= stepBeforeCompute)
        {
            SetNewTargetPosition();
            stepCount = 0;
        }

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
            }
        }
        else 
        {
            //Debug.Log("else");
            SetNewTargetPosition();
        }
        base.SetDestination(currentTargetPosition);
    }

    public void SetMode(Enemy_State state)
    {
        if (useCooperative)
        {
            pathManager.SetMode(state);
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

}
