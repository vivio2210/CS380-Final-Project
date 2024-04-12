using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Camera Camera = null;
    private NavMeshAgent Agent;

    private RaycastHit[] Hits = new RaycastHit[1];

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
                Agent.SetDestination(PositionConverter.GridPosToWorld(PositionConverter.WorldToGridPos(Hits[0].point)));
            }
        }



        //Debug.Log(gameObject.transform.forward.normalized);
    }
}
