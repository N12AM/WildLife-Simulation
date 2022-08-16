using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    public Slider slider;
   
    public void SetMaxHunger(float hunger)
    {
        slider.maxValue = hunger;
        
        // 0 because initially hunger is 0
        slider.value = 0;
    }
   
    public void SetCurrentHunger(float hunger)
    {
        slider.value = hunger;
    }
}
