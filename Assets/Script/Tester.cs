using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Tester : MonoBehaviour
{
    [SerializeField]
    private Camera Camera = null;
    private NavMeshAgent Agent;

    private RaycastHit[] Hits = new RaycastHit[1];

    private List<Gridpos> currentPath = new List<Gridpos>();

    [SerializeField]
    private Vector3 currentTargetPosition = new Vector3(0, 0, 0);

    [SerializeField]
    private Gridpos currentGridPosition = new Gridpos(0, 0);

    [SerializeField]
    private int StartX = 0;
    [SerializeField]
    private int StartZ = 0;

    [SerializeField]
    private int GoalX = 0;
    [SerializeField]
    private int GoalZ = 0;

    private void Start()
    {
        AStarPather.initialize();
    }

    [ContextMenu("ComputePath")]
    public void ComputePath()
    {
        Gridpos start = new Gridpos(StartX, StartZ);
        Gridpos goal = new Gridpos(GoalX, GoalZ);
        List<Gridpos> result = AStarPather.compute_path(start, goal);
        for (int i = 0; i < result.Count; i++)
        {
            Debug.Log("(" + result[i].posx + ", " + result[i].posz + ")");
        }
    }
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, Hits) > 0)
            {
                currentPath = AStarPather.compute_path(PositionConverter.WorldToGridPos(gameObject.transform.position), PositionConverter.WorldToGridPos(Hits[0].point));
                currentGridPosition = new Gridpos(currentPath[0].posx, currentPath[0].posz);
                currentPath.RemoveAt(0);
                currentTargetPosition = PositionConverter.GridPosToWorld(currentGridPosition);
                Agent.SetDestination(currentTargetPosition);
            }
        }

        if (currentPath.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentGridPosition = currentPath[0];
                currentPath.RemoveAt(0);
                currentTargetPosition = PositionConverter.GridPosToWorld(currentGridPosition);
            }
            Agent.SetDestination(currentTargetPosition);
        }
    }

}
