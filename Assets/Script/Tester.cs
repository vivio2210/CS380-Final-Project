using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Tester : MoveableAgent
{
    [SerializeField]
    private Camera Camera = null;
    private RaycastHit[] Hits = new RaycastHit[1];
    private List<Gridpos> currentPath = new List<Gridpos>();
    private Vector3 currentTargetPosition = new Vector3(0, 0, 0);
    private Gridpos currentGridPosition = new Gridpos(0, 0);
    private Gridpos currentTargetGridPosition = new Gridpos(0, 0);
    private Vector3 currentFacingDirection = new Vector3(0, 0, 1);

    private void Awake()
    {
        AStarPather.initialize();
        currentTargetPosition = transform.position;
    }
    private void Update()
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
                    currentPath.RemoveAt(0);
                    currentTargetPosition = PositionConverter.GridPosToWorld(currentTargetGridPosition);
                    base.SetDestination(currentTargetPosition);

                    currentGridPosition = PositionConverter.WorldToGridPos(gameObject.transform.position);
                    currentFacingDirection = new Vector3(currentTargetGridPosition.posx - currentGridPosition.posx, 0, currentTargetGridPosition.posz - currentGridPosition.posz).normalized;
                }
            }
        }

        if (currentPath.Count > 0)
        {
            if (math.abs(currentTargetPosition.x - gameObject.transform.position.x) <= 0.1f && math.abs(currentTargetPosition.z - gameObject.transform.position.z) <= 0.1f)
            {
                currentTargetGridPosition = currentPath[0];
                currentPath.RemoveAt(0);
                currentTargetPosition = PositionConverter.GridPosToWorld(currentTargetGridPosition);

                currentGridPosition = PositionConverter.WorldToGridPos(gameObject.transform.position);
                currentFacingDirection = new Vector3(currentTargetGridPosition.posx - currentGridPosition.posx, 0, currentTargetGridPosition.posz - currentGridPosition.posz).normalized;
            }
            base.SetDestination(currentTargetPosition);
        }
        base.SetDestination(currentTargetPosition);
    }

}
