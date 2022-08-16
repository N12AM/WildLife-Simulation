using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;

public class PlantsAndFruits : MonoBehaviour
{
    // [Range(0,1)]
    public int availableFood = 10;

    [SerializeField] private TagSelector.ScanningTag scanningLayer;
    public int ConsumePlant(int amount)
    {
        if (availableFood > 0)
        {
            availableFood -= amount;
            return amount;
        }
        else
        {
            availableFood = 0;
            Destroy(gameObject);
            // Destroy(gameObject);
            return 0;
        }

    }

    private void Update()
    {
       Something(); 
    }

    private void Something()
    {
        if ("Herbivour" == scanningLayer.ToString())
        {
            print("what up");
        }
        // print(scanningLayer);
    }
}
