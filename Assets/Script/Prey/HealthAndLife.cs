using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAndLife : MonoBehaviour
{
    [Tooltip("'0' means close to death and '10' means full health")]
    [Range(0, 1000)] [SerializeField] private int health = 100;

    [SerializeField] private HealthBar healthBar;
    public bool isPredatorAlertTriggered;
    public int Health
    {
        get => health;
        set => health = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int ConsumeFood(int amount)
    {
        if (health > 0)
        {
            health -= amount;
            if (healthBar != null)
            {
                healthBar.SetCurrentHealth(health);
            }
            return amount;
        }
        else
        {
            health = 0;
            Destroy(gameObject);
            // Destroy(gameObject);
            return 0;
        }

    }

    public void IncreaseOrDecreaseHealth(int _health)
    {
        // we are not checking if health is > 0
        // because we are already checking it in CarnivorousFoodBehaviourController class
        health += _health;
        if (healthBar != null)
        {
            healthBar.SetCurrentHealth(health);
        } 
    }
}
