using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    public NavMeshAgent agent;
    public LayerMask ground;
    public LayerMask clickable;

    private Unit ParentUnit;
    private bool harvestor;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        ParentUnit = this.GetComponentInParent<Unit>();
        harvestor = ParentUnit.Harvestor;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (harvestor && Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (hit.collider.CompareTag("Resource"))
                {
                    ResourceProducer resource = hit.collider.GetComponentInParent<ResourceProducer>();
                    if (ResourceManager.Instance.IsResourceDiscovered(resource))
                        ParentUnit.StartCollection(resource);
                }
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}