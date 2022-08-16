using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehaviour : MonoBehaviour
{
    [SerializeField] private bool displayGizmos = true;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float predatorDetectionRange = 10f;

    
    // Start is called before the first frame update
    void Start()
    {

    }


    private void OnDrawGizmos()
    {
        if (!displayGizmos)
        {
            return;
        }
        var position = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, predatorDetectionRange);
    }
}
