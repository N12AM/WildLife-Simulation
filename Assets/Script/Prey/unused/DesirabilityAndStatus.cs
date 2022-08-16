using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesirabilityAndStatus : MonoBehaviour
{
    [Tooltip("'0' means not hungry and '1' means full hungry")]
    [Range(0,1)] [SerializeField] private float hungerDesirability = 0;

    private HealthAndLife _healthAndStamina;
    // Start is called before the first frame update
    void Start()
    {
        _healthAndStamina = GetComponent<HealthAndLife>();
    }

    private IEnumerator EnableHungerBehaviour()
    {
        for (;;)
        {
            yield return new WaitForSeconds(10f);

            //increase hunger over time
            if (hungerDesirability < 1)
            {
                hungerDesirability += 0.3f;
            }
            
            else
            {
                //when hunger is full decrease health
                if (_healthAndStamina.Health > 0)
                {
                    _healthAndStamina.Health -= 2;
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

    // -------------------------------------------public methods------------------------------------starts----------
    public void ConsumeFood()
    {
        
    }
}
