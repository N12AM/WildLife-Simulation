using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class Manager : MonoBehaviour
{
       private enum HudToggle
       {
             On,
             Off
       }
       [SerializeField] private int hudObjects;

       [SerializeField] private HudToggle toggleAllHud = HudToggle.On;

       private bool _offValueChanged = false;
       private bool _onValueChanged = true;

       private GameObject[] _objects;
    // Start is called before the first frame update
    void Start()
    {
           _objects = GameObject.FindGameObjectsWithTag("hud");
 
    }

    // Update is called once per frame
    void Update()
    {
           hudObjects = _objects.Length;

           if (toggleAllHud == HudToggle.Off)
           {
                  if (_offValueChanged == true)
                  {
                         // _objects = GameObject.FindGameObjectsWithTag("hud");
                         for (int i = 0; i < _objects.Length ; i++)
                         {
                                _objects[i].SetActive(false);
                         }

                         _offValueChanged = false;
                         _onValueChanged = true;
                         
                         print("off");
                  }
           } 
           if (toggleAllHud == HudToggle.On)
           {
                  if (_onValueChanged == true)
                  {
                         // _objects = GameObject.FindGameObjectsWithTag("hud");
                         for (int i = 0; i < _objects.Length ; i++)
                         {
                                _objects[i].SetActive(true);
                         }

                         _onValueChanged = false;
                         _offValueChanged = true;
                         print("on");
                  }
           }

           // var name = "";
           // for (int i = 0; i < _objects.Length; i++)
           // {
           //        name = string.Format("{0}{1}", name, _objects[i].name); 
           //        Debug.Log(name);
           // }
           // Debug.Log(hudObjects); 
    }
}
