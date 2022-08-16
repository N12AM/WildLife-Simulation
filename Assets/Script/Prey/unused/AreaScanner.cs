using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScanner : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float scanDelay = 0.2f;
    [SerializeField] private bool displayGizmos = true;

    public Collider[] _scannedColliders;

    private IEnumerator _areaScanCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        _areaScanCoroutine = StartAreaScanning();
        StartCoroutine(_areaScanCoroutine);
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos)
        {
            return;
        }
        Gizmos.color = Color.green;
        // var position = transform.position;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    private IEnumerator StartAreaScanning()
    {
        for (;;)
        {
            _scannedColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            yield return new WaitForSeconds(scanDelay);
        }
    }

    public Collider[] GetAreaScannedColliders(string colliderTag)
    {
        
        // the following code creates a copy of the '_collider' array after filtering out the data
        // then returns the new array
        
        // Collider[] colliders = new Collider[] { };
        // int index = 0;
        // for (int i = 0; i < _scannedColliders.Length; i++)
        // {
        //     if (_scannedColliders[i].CompareTag(colliderTag))
        //     {
        //         colliders[index] = _scannedColliders[i];
        //         index++;
        //     }
        // }
        // return colliders;
        
        //---------------------------------------------------------------------------------------------------

        return _scannedColliders;
    }

}
