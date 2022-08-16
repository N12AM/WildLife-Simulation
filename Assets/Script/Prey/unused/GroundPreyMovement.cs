using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundPreyMovement : MonoBehaviour
{
    public Camera _camera;

    private NavMeshAgent _agent;
    [SerializeField] private float agentStoppingDistance = 0.5f; 

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = agentStoppingDistance;
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // private void FixedUpdate()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         var ray = _camera.ScreenPointToRay(Input.mousePosition);
    //         RaycastHit hit;
    //
    //         if (Physics.Raycast(ray, out hit))
    //         {
    //             agent.SetDestination(hit.point);
    //         }
    //     } 
    // }

    public void MovePreyAgent(Vector3 destination)
    {
        var position = transform.position;
        Debug.DrawLine(position, destination);

        // float minDistance = Vector3.Distance(position, destination); 
        // if ( !( minDistance <= 1f))
        // {
            _agent.SetDestination(destination);
        // }
        // else
        // {
            // var f = _agent.stoppingDistance;
        // }
    }
}
