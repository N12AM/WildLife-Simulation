using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;

public class NormalMovementBehaviourModel : MonoBehaviour
{
    [SerializeField] private Vector3 lineCastOffset = new Vector3(0,2,0);
       [SerializeField] private Vector3 endPointOffset = new Vector3(0,0,0);
       [SerializeField] private float rayMaxDistance = 10f;
       [SerializeField] private float rayHeight = -4;
       [SerializeField] private float agentStoppingDistance = 0.2f; 
 
       [Tooltip("the height is controlled by 'RayHeight'")]
       [SerializeField] private Vector3[] forwardRayDirection = new[]
                                                                {
                                                                       new Vector3(0,0,10),
                                                                       new Vector3(7, 0, 7),
                                                                       new Vector3(-7, 0, 7),
                                                                       new Vector3(10, 0, 0),
                                                                       new Vector3(-10, 0, 0)
                                                                };
       [SerializeField] private Vector3[] backwardRayDirection = new[]
                                                                {
                                                                       new Vector3(0,0,-10),
                                                                       new Vector3(7, 0, -7),
                                                                       new Vector3(-7, 0, -7)
                                                                };

       [SerializeField] private bool showRayDirection = true;

       [SerializeField] private float raycastDelay = 1f;

       private int forwardRayCount = 0;
       private int backwardRayCount = 0;
       private bool _isTHeDestinationSet = false;

       private IEnumerator _directionalMovementCoroutine;
       
       private NavMeshAgent _agent;
       
    // Start is called before the first frame update
    void Start()
    {
           forwardRayCount = forwardRayDirection.Length;
           backwardRayCount = backwardRayDirection.Length;

           _agent = GetComponent<NavMeshAgent>();

           for (int i = 0; i < forwardRayCount; i++)
           {
                  forwardRayDirection[i].y = rayHeight;
           }
           for (int i = 0; i < backwardRayCount; i++)
           {
                  backwardRayDirection[i].y = rayHeight;
           }
           
           //make sure to add the stopping distance otherwise agent will go inside the target
           //that will through an error of "index out of bounds"
           //add a distance that keeps the agent outside the target collider bound
           _agent.stoppingDistance = agentStoppingDistance;

           if (_directionalMovementCoroutine == null)
           {
                  _directionalMovementCoroutine = StartDirectionalRayCast();
                  StartCoroutine(_directionalMovementCoroutine);
           }
    }

    //----------------------------------private Methods------------------------------------------------
     private void CastForwardRays()
    {
           //find all surface points where the ray actually hits
           for (int i = 0; i < forwardRayCount; i++)
           {
                  //check if probability the ray should be fired
                  //if the probability does not match, then skip this iteration to the next one
                  if (GetForwardRaysProbability() != i)
                  {
                         continue;;
                  }
                  RaycastHit hit;
                  bool isSurfaceReachable = Physics.Raycast(transform.position + lineCastOffset,
                                                            transform.TransformDirection(forwardRayDirection[i]) + endPointOffset,
                                                            out hit, rayMaxDistance);
                  if (showRayDirection)
                  {
                         Debug.DrawRay(transform.position + lineCastOffset, transform.TransformDirection(forwardRayDirection[i]) + endPointOffset, Color.cyan);
                  }
                  if (isSurfaceReachable)
                  {
                         if (hit.collider.CompareTag("GroundSurface"))
                         {
                                // Debug.DrawLine(transform.position + lineCastOffset, hit.point, Color.yellow);
                                MoveAgent(hit.point);
                                       
                                //if the ray touches the ground then stop the current loop
                               break;
                         }
                  }
           }
    }

    private void CastBackwardRays()
    {
           //find all surface points where the ray actually hits
           for (int i = 0; i < backwardRayCount; i++)
           {
                  //check if probability the ray should be fired
                  //if the probability does not match, then skip this iteration to the next one
                  if (GetBackwardRaysProbability() != i)
                  {
                         continue;
                  }
                  RaycastHit hit;
                  bool isSurfaceReachable = Physics.Raycast(transform.position + lineCastOffset,
                                                            transform.TransformDirection(backwardRayDirection[i]) + endPointOffset,
                                                            out hit, rayMaxDistance);
                  if (showRayDirection)
                  {
                         Debug.DrawRay(transform.position + lineCastOffset, transform.TransformDirection(backwardRayDirection[i]) + endPointOffset, Color.cyan);
                  }
                  if (isSurfaceReachable)
                  {
                         if (hit.collider.CompareTag("GroundSurface"))
                         {
                                // Debug.DrawLine(transform.position + lineCastOffset, hit.point, Color.yellow);
                                MoveAgent(hit.point);
                                
                                //if the ray touches the ground then stop the current loop
                                break;
                         }
                  }
           } 
    }
   
    //---------------------------------- probability Check --------------------------------------------
    /// <summary>
    /// returns 'true' for forward direction
    /// </summary>
    /// <returns></returns>
    private bool ForwardOrBackwardRayProbability()
    {
           var randomValue = Random.value;
           if (randomValue < 0.85)
           {
                  return true;
           }

           return false;
    }

    private int GetForwardRaysProbability()
    {
           float probability = Random.value;
           
           //probability for 0 degree forward 
           if (probability <= 0.5)
           {
                  return 0;
           }
           
           //probability for 45 degree forward-right 
           if (probability <= 0.7)
           {
                  return 1;
           }
           //probability for -45 degree forward-left 
           if(probability <= 0.9)
           {
                  return 2;
           }
           
           //probability for 90 degree right
           if (probability <= .95)
           {
                  return 3;
           }
           
           //probability for -90 degree left 
           return -2;
    }
    private int GetBackwardRaysProbability()
    {
           float probability = Random.value;
           //probability for 135 degree backward-right 
           if (probability <= 0.4)
           {
                  return 0;
           } 
           
           //probability for -135 degree backward-left 
           if (probability <= 0.8)
           {
                  return 1;
           }
           
           //probability for -180 degree backward
           return -2;
    }

    //----------------------------------private Coroutines --------------------------------------------

    private IEnumerator StartDirectionalRayCast()
    {
           for (;;)
           {
                  var forwardProbability = ForwardOrBackwardRayProbability();

                  if (forwardProbability)
                  {
                         CastForwardRays();
                  }
                  else
                  {
                         CastBackwardRays();
                  }

                  yield return new WaitForSeconds(raycastDelay);
           }
    }

    
    // -------------------------------------------public methods------------------------------------starts----------
    
    /// <summary>
    /// Disable random directional movement
    /// </summary>
    public void StopRandomDirectionMovement()
    {
           if (_directionalMovementCoroutine != null)
           { 
                  StopCoroutine(_directionalMovementCoroutine);
                  _directionalMovementCoroutine = null;
           }
    }

    /// <summary>
    /// Enable Random direction movement
    /// </summary>
    public void StartRandomDirectionalMovement()
    {
           if (_directionalMovementCoroutine == null)
           {
                  _directionalMovementCoroutine = StartDirectionalRayCast();
                  StartCoroutine(_directionalMovementCoroutine);
           }
    }
    
    /// <summary>
    /// Move the agent to the desired world position
    /// </summary>
    /// <param name="destination"></param>
    public void MoveAgent(Vector3 destination)
    {
           var position = transform.position;
           Debug.DrawLine(position, destination, Color.black);

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
