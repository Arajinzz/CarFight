using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public GameObject[] SuspensionPoints;
    public float SuspensionDistance = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        foreach( GameObject suspensionPt in SuspensionPoints )
        {
            Vector3 direction = - transform.TransformDirection(suspensionPt.transform.up);
            Ray ray = new Ray(suspensionPt.transform.position, direction);
            Debug.DrawRay(ray.origin, direction * SuspensionDistance, Color.green);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, SuspensionDistance))
            {
                Debug.Log(hit.collider.name);                
            }

        }

    }
}
