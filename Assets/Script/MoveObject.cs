using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
       public float forwardSpeedMultiplier = 5f;

       public float sideSpeedMultiplier = 2f;
       // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
           if (Input.GetKey("w"))
           {
                 transform.Translate(Vector3.forward* forwardSpeedMultiplier); 
           }
           if (Input.GetKey("s"))
           {
                  transform.Translate(Vector3.back* forwardSpeedMultiplier); 
           } 
           if (Input.GetKey("a"))
           {
                  transform.Translate(Vector3.left* forwardSpeedMultiplier); 
           } 
           if (Input.GetKey("d"))
           {
                  transform.Translate(Vector3.right* forwardSpeedMultiplier); 
           } 
    }
}
