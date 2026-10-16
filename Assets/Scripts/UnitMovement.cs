using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    public NavMeshAgent agent;
    public LayerMask ground;
    public LayerMask clickable;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (hit.collider.CompareTag("Resource"))
                {
                    AttemptCollection();
                }
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    private void AttemptCollection()
    {
        throw new NotImplementedException();
    }
}