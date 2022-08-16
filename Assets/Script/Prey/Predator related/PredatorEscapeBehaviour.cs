using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PredatorEscapeBehaviour : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask layersToScan;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float scanDelay = 1f;
    [SerializeField] private float destinationLocateDelay = 0.5f;
    [SerializeField] private Vector3[] rayDirection = new[]
                                                             {
                                                                 new Vector3(0,0,10),
                                                                 new Vector3(7, 0, 7),
                                                                 new Vector3(-7, 0, 7),
                                                                 new Vector3(10, 0, 0),
                                                                 new Vector3(-10, 0, 0),
                                                                 new Vector3(0,0,-10),
                                                                 new Vector3(7, 0, -7),
                                                                 new Vector3(-7, 0, -7)
                                                              };
    [SerializeField] private Vector3 lineCastOffset = new Vector3(0,2,0);
    [SerializeField] private Vector3 endPointOffset = new Vector3(0,0,0);
    [SerializeField] private float rayMaxDistance = 10f;
    [SerializeField] private float rayHeight = -4;
    public Collider[] colliders;
    private int _rayCount = 0;
    private List<Vector3> _destinations;

    private IEnumerator _predatorEscapeCoroutine;

    private float timeSpent = 0;
    // private Vector3 _targetDEstination
    
    private NormalMovementBehaviourModel _normalMovement;

    private FoodBehaviourModel _foodBehaviour;

    private HealthAndLife _healthAndLife;
    // Start is called before the first frame update
    void Start()
    {
        _rayCount = rayDirection.Length;
        _destinations = new List<Vector3>();

        _healthAndLife = GetComponent<HealthAndLife>();
        _normalMovement = GetComponent<NormalMovementBehaviourModel>();
        _foodBehaviour = GetComponent<FoodBehaviourModel>();
        
        for (int i = 0; i < _rayCount; i++)
        {
            rayDirection[i].y = rayHeight;
        }
        StartCoroutine(ScanForPossiblePredator());
    }

    // Update is called once per frame
    void Update()
    {
        timeSpent += Time.deltaTime;
        if (timeSpent > 5f && _healthAndLife.isPredatorAlertTriggered)
        {
            _healthAndLife.isPredatorAlertTriggered = false;
            if (_predatorEscapeCoroutine != null)
            {
                StopCoroutine(_predatorEscapeCoroutine);
                _predatorEscapeCoroutine = null; 
            }
            _normalMovement.StartRandomDirectionalMovement();
            _foodBehaviour.StartFoodDetectionSystem();
            
        }
    }

    private IEnumerator ScanForPossiblePredator()
    {
        for (;;)
        {
            RaycastHit hit;
            var detectedObjects = Physics.OverlapSphere(transform.position, detectionRadius, layersToScan);
            colliders = detectedObjects;
            if (detectedObjects.Length > 0)
            {
                _normalMovement.StopRandomDirectionMovement();
                _foodBehaviour.StopFoodDetectionSystem();

                // trigger the predator alarm
                _healthAndLife.isPredatorAlertTriggered = true;
                
                // reset time spend in between
                timeSpent = 0;

                // if (_predatorEscapeCoroutine == null)
                // {
                    // _predatorEscapeCoroutine = MoveToPosition(detectedObjects);
                    // StartCoroutine(_predatorEscapeCoroutine);
                // }
                MoveToPosition(detectedObjects);
            }
            yield return new WaitForSeconds(destinationLocateDelay);
            
        }
    }

    private void MoveToPosition(Collider[] colliders)
    {
        _destinations.Clear();
        //find all surface points where the ray actually hits
        for (int i = 0; i < _rayCount; i++)
        {

            RaycastHit hit;
            bool isSurfaceReachable = Physics.Raycast(transform.position + lineCastOffset,
                                                      transform.TransformDirection(rayDirection[i]) + endPointOffset,
                                                      out hit, rayMaxDistance, groundLayer);
            // if (showRayDirection)
            // {
            Debug.DrawRay(transform.position + lineCastOffset, transform.TransformDirection(rayDirection[i]) + endPointOffset, Color.cyan);
            // }
            if (isSurfaceReachable)
            {
                Debug.Log("hit ground");
                _destinations.Add(hit.point);
            }
        }

        int enemyCount = colliders.Length;
        Vector3 referenceEnemy = colliders[0].transform.position;
        
        Debug.Log("count: "+enemyCount);
        float closestEnemyDistance = Vector3.Distance(transform.position, referenceEnemy);
        
        if (enemyCount > 1)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
                if (distance < closestEnemyDistance)
                {
                    closestEnemyDistance = distance;
                    referenceEnemy = colliders[i].transform.position;
                    Debug.Log("Ref: "+referenceEnemy);
                }
            } 
        }
        int length = _destinations.Count;
        if (length > 0)
        {
            Debug.Log("len: "+length);

            float maxDistance = Vector3.Distance(referenceEnemy, _destinations[0]);
            var targetDestination = _destinations[0];
            
            for (int i = 0; i < length; i++)
            {
                float distance = Vector3.Distance(referenceEnemy, _destinations[i]); 
                if ( distance > maxDistance)
                {
                    maxDistance = distance;
                    targetDestination = _destinations[i];
                    Debug.Log("de: "+targetDestination);

                }
            }
            
            print("go");
            _normalMovement.MoveAgent(targetDestination);
        }

        // yield return new WaitForSeconds(destinationLocateDelay);
        
        
    }
}
