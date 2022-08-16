using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDetectionBehaviour : MonoBehaviour
{
    [SerializeField] private bool displayGizmos = true;
    [SerializeField] private float foodDetectionRange = 5f;
    [SerializeField] private LayerMask foodDetectionLayerMask;

    private GroundPreyMovement _preyAgentMovement;
    private BasicMovement _basicAgentMovement;

    private List<Vector3> _detectableObjects;

    private Vector3 _pointOfhit;

    private IEnumerator _startFoodDetectionCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        _preyAgentMovement = GetComponent<GroundPreyMovement>();
        _basicAgentMovement = GetComponent<BasicMovement>();
        _detectableObjects = new List<Vector3>();

        _startFoodDetectionCoroutine = StartFoodDetection();
        StartCoroutine(StartFoodDetection());
    }

   
    
    private IEnumerator StartFoodDetection()
    {
        for (;;)
        {
            RaycastHit hit;
            var detectedObjects = Physics.OverlapSphere(transform.position, foodDetectionRange, foodDetectionLayerMask);
            int length = detectedObjects.Length;
            if (length > 0)
            {
                _basicAgentMovement.StopRandomDirectionMovement();
                _preyAgentMovement.MovePreyAgent(FindVisibleNearestObject(detectedObjects));
            }
            
            //no need to use detection every frame; so we add a delay 
            yield return new WaitForSeconds(0.667f);
            _basicAgentMovement.StartRandomDirectionalMovement();
        }
    }
    private bool IsTheObjectInLineOfSight(Vector3 targetPosition)
    {
            RaycastHit ray;
            var isDetectedObjectInLIneOfSight = Physics.Linecast(transform.position, targetPosition, out ray);
            if (isDetectedObjectInLIneOfSight && ray.collider.CompareTag("PlantsAndFruits")) //&& ray.collider.name.Equals(name))
            {
                //where the ray actually hits the collider
                _pointOfhit = ray.point;
                return true;
            }

            return false;
    }
    
    
    private Vector3 FindVisibleNearestObject(Collider[] colliders)
    {


       
        //find all objects which are detectable and visible to the animal
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 targetPosition = colliders[i].transform.position;
            if (IsTheObjectInLineOfSight(targetPosition))
            {
                // _detectableObjects.Add(targetPosition);
                _detectableObjects.Add(_pointOfhit);
            }
        } 
        
        //find the closest object and set it as destination for agent
        Vector3 position = transform.position;
        
        Vector3 firstElementPosition = _detectableObjects[0];
        float minDistance = Vector3.Distance(position,firstElementPosition);
        Vector3 targetDestination = firstElementPosition;
        
        for (int i = 0; i < _detectableObjects.Count; i++)
        {
            float currentDistance = Vector3.Distance(position, _detectableObjects[i]);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                targetDestination = _detectableObjects[i];
            } 
        }

        //reset the list to use it again
        _detectableObjects.Clear();
        return targetDestination;
    } 
    
    private void OnDrawGizmos()
    {
        if (!displayGizmos)
        {
            return;
        }
        Gizmos.color = Color.green;
        var position = transform.position;
        Gizmos.DrawWireSphere(position, foodDetectionRange);
    }

    // -------------------------------------------public methods------------------------------------starts----------
    /// <summary>
    /// Enable the food Detection system
    /// </summary>
    public void StartFoodDetectionSystem()
    {
        if (_startFoodDetectionCoroutine == null)
        {
            _startFoodDetectionCoroutine = StartFoodDetection();
            StartCoroutine(_startFoodDetectionCoroutine); 
        }
        
    }

    /// <summary>
    /// Disable the food detection system
    /// </summary>
    public void StopFoodDetectionSystem()
    {
        if (_startFoodDetectionCoroutine != null)
        {
           StopCoroutine(_startFoodDetectionCoroutine);
           _startFoodDetectionCoroutine = null;
        }
    }
}
