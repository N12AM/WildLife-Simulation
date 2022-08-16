using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSelector : MonoBehaviour
{
       public enum ScanningTag
       {
              PlantsAndFruits,
              GroundAnimal,
              GroundPreyAnimal,
              GroundSurface,
              Herbivour,
              Carnivorous
              
       }

       public ScanningTag _tag;
}
