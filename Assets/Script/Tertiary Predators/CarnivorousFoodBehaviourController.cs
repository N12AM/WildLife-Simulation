using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarnivorousFoodBehaviourController : MonoBehaviour
{
       [SerializeField] private bool displayGizmos = true;
    public Collider[] colliders;
    [SerializeField] private float foodDetectionRange = 5f;
    [SerializeField] private LayerMask foodDetectionLayerMask;
    [SerializeField] private TagSelector.ScanningTag foodTagToSearch;
    [Tooltip("'0' means not hungry and '10' means full hungry")]
    [Range(0,10)] [SerializeField] private int hungerDesirability = 0;

    [Tooltip("define the rate at which the animal should eat plant or fruit")]
    [SerializeField] private int foodEatingRate = 1;

    [SerializeField] private int hungerIncreaseRate = 3;

    [SerializeField] private float foodDetectionSamplingRate = 0.667f;
    [SerializeField] private float hungerDelay = 10f;

    [SerializeField] private Material _makedMaterial;
    private NormalMovementBehaviourModel _movementBehaviourModel;
    private HealthAndLife _selfHealth;
    private HealthAndLife _foodHealth;
    private NavMeshAgent _agent;
    
    private List<Vector3> _detectedPreys;
    private Vector3 _pointOfHit;
    private GameObject _markedPrey = null;
    private IEnumerator _startFoodDetectionCoroutine;

    private HungerBar _hungerBar;
    
    // Start is called before the first frame update
    void Start()
    {
        _movementBehaviourModel = GetComponent<NormalMovementBehaviourModel>();
        _selfHealth = GetComponent<HealthAndLife>();
        _agent = GetComponent<NavMeshAgent>();

        _hungerBar = GetComponentInChildren<HungerBar>();
        if (_hungerBar != null)
        {
            _hungerBar.SetMaxHunger(10);
        }

        _detectedPreys = new List<Vector3>();

        // _startFoodDetectionCoroutine = StartFoodDetection();
        // StartCoroutine(StartFoodDetection());

        StartCoroutine(EnableHungerBehaviour());
    }

    //----------------------------------private Coroutines --------------------------------------------
    private IEnumerator StartFoodDetection()
    {
        for (;;)
        {
            RaycastHit hit;
            var detectedObjects = Physics.OverlapSphere(transform.position, foodDetectionRange, foodDetectionLayerMask);
            colliders = detectedObjects;
            // the search tag will be selected from the unity inspector
            // var detectedObjects = _areaScanner.GetAreaScannedColliders(foodTagToSearch.ToString());
            int length = detectedObjects.Length;
            if (length > 0)
            {
                _movementBehaviourModel.StopRandomDirectionMovement();
                
                // set the mark on a prey
                if (_markedPrey == null)
                {
                    FindVisibleNearestObject(detectedObjects); 
                }
                else
                {
                    // chase after the marked prey
                    _movementBehaviourModel.MoveAgent(_markedPrey.transform.position);
                }
                    // _movementBehaviourModel.MoveAgent(FindVisibleNearestObject(detectedObjects)); 
                
               
                ConsumeFood();
            }
            
            //no need to use detection every frame; so we add a delay 
            yield return new WaitForSeconds(foodDetectionSamplingRate);
            _movementBehaviourModel.StartRandomDirectionalMovement();
        }
    } 


    //----------------------------------private Methods------------------------------------------------
    //-------------------------------------------------------------------------------------------------
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

    private int RandomPreySelector(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
    
    //---------------------------------------------------------------------------------------------
    //------------------------------------------------detect food----------------------------------
    //---------------------------------------------------------------------------------------------
    private bool IsTheObjectInLineOfSight(Vector3 targetPosition)
    {
        RaycastHit ray;
        var isDetectedObjectInLIneOfSight = Physics.Linecast(transform.position, targetPosition, out ray);
        // if (isDetectedObjectInLIneOfSight && ray.collider.CompareTag("PlantsAndFruits")); //&& ray.collider.name.Equals(name))
        if (isDetectedObjectInLIneOfSight && ray.collider.CompareTag(foodTagToSearch.ToString())); //&& ray.collider.name.Equals(name))
        {
            //where the ray actually hits the collider
            _pointOfHit = ray.point;
            return true;
        }

        return false;
    } 
    
    private void FindVisibleNearestObject(Collider[] colliders)
    {
        // //find all objects which are detectable and visible to the animal
        // for (int i = 0; i < colliders.Length; i++)
        // {
        //     Vector3 targetPosition = colliders[i].transform.position;
        //     if (IsTheObjectInLineOfSight(targetPosition))
        //     {
        //         // _detectedPreys.Add(targetPosition);
        //         _detectedPreys.Add(_pointOfHit);
        //     }
        // } 
        //
        // //find the closest object and set it as destination for agent
        // Vector3 position = transform.position;
        //
        // Vector3 firstElementPosition = _detectedPreys[0];
        // float minDistance = Vector3.Distance(position,firstElementPosition);
        // Vector3 targetDestination = firstElementPosition;
        //
        // for (int i = 0; i < _detectedPreys.Count; i++)
        // {
        //     float currentDistance = Vector3.Distance(position, _detectedPreys[i]);
        //     if (currentDistance < minDistance)
        //     {
        //         minDistance = currentDistance;
        //         targetDestination = _detectedPreys[i];
        //     } 
        // }
        //
        // RaycastHit hit;
        // var script = Physics.Linecast(transform.position, targetDestination, out hit);
        // if (script)
        // {
        //     _foodHealth = hit.collider.GetComponent<HealthAndLife>();
        // }
        //
        // if (transform.CompareTag("Carnivorous"))
        // {
        //     Debug.Log("food: "+hit.collider.name );
        // }
        //reset the list to use it again
        // _detectedPreys.Clear();
        
        _markedPrey = colliders[RandomPreySelector(0, colliders.Length)].gameObject;
        
        //get reference to the health data of the marked prey
        _foodHealth = _markedPrey.GetComponent<HealthAndLife>();
        _markedPrey.GetComponentInChildren<MeshRenderer>().material = _makedMaterial;
        // return colliders[RandomPreySelector(0, colliders.Length)].transform.position;
    } 
    
    


    // private void Update()
    // {
    //     var f = Vector3.Distance(transform.position, _pointOfHit);
    //     Debug.Log(f);
    // }
    
    
    //------------------------------------------------hunger behavior----------------------------------
    
    private IEnumerator EnableHungerBehaviour()
    {
        for (;;)
        {
            yield return new WaitForSeconds(hungerDelay);

            if (hungerDesirability == 0)
            {
                hungerDesirability += hungerIncreaseRate -1;
                if (_hungerBar != null)
                {
                    _hungerBar.SetCurrentHunger(hungerDesirability);
                }

                if (_startFoodDetectionCoroutine != null)
                {
                    StopCoroutine(_startFoodDetectionCoroutine);
                    _startFoodDetectionCoroutine = null;
                    
                    _movementBehaviourModel.StartRandomDirectionalMovement();
                    print("zero stop");
                } 
            }
            //increase hunger over time
            else if (hungerDesirability < 10 && hungerDesirability > 0)
            {
                hungerDesirability += hungerIncreaseRate;
                if (_hungerBar != null)
                {
                    _hungerBar.SetCurrentHunger(hungerDesirability);
                }
                //start hunting for food
                if (_startFoodDetectionCoroutine == null)
                {
                    _startFoodDetectionCoroutine = StartFoodDetection();
                    StartCoroutine(_startFoodDetectionCoroutine);
                    print("start");
                }
            }
            
            else
            {
                //when hunger is full decrease health
                if (_selfHealth.Health > 0)
                {
                    // _selfHealth.Health -= 2;
                    _selfHealth.IncreaseOrDecreaseHealth(-2);
                }
                else
                {
                    DeathDueToHunger();
                }
            }
        }
    } 
    private void DeathDueToHunger()
    {
        //instantiate a death body in place of the actual body
        
        //death body inherits the nutritional values of parent (meat and others)
        
        //death body inherits the diseases
        
        //set death body health
        
        //destroy the current live animal
        Destroy(gameObject);
    }

    public void ConsumeFood()
    {
        //check if agent is hungry
        if (hungerDesirability > 0)
        {
            // float distance = Vector3.Distance(transform.position, _pointOfHit);
            float distance = Vector3.Distance(transform.position, _markedPrey.transform.position);
            if (distance <= _agent.stoppingDistance + 1)
            {

                int foodToEat = 0;
                //when the plant dies the script will be null for other animals targeting the same plant
                //so we need to check for null
                if (_foodHealth != null)
                {
                    //eat food
                    foodToEat = _foodHealth.ConsumeFood(foodEatingRate);
                }
                else
                {
                    foodToEat = 0;
                }
                hungerDesirability -= foodToEat;
                if (_hungerBar != null)
                {
                    _hungerBar.SetCurrentHunger(hungerDesirability);
                }
                print("decrease");
                
                //decrease food amount 
                
                //decrease hunger
            }

        }
        
        //if hungry then eat
        
        //enable health recovery
        
        //increase health
        
        //if not hungry enable random movement and disable food hunt
        else
        {
            if (_startFoodDetectionCoroutine != null)
            {
                StopCoroutine(_startFoodDetectionCoroutine);
                _startFoodDetectionCoroutine = null;
                // print("stop");
            }
        }
        
        
    } 
    
    
    //------------------------------------------------detect food----------------------------------
    //------------------------------------------------detect food----------------------------------
    //------------------------------------------------detect food----------------------------------
    
    
    
    
    //--------------------------------------------------------------------------------------------------
    // -------------------------------------------public methods------------------------------------starts----------
    //----------------------------------Coroutine Initialize ----------------------------------

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
