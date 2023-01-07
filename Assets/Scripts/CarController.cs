using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public GameObject[] SuspensionPoints;
    public float SuspensionDistance = 0.5f;

    private Ray[] SuspensionDebugRays;
    private RaycastHit[] SuspensionDebugHit;


    // Start is called before the first frame update
    void Start()
    {

        SuspensionDebugHit = new RaycastHit[SuspensionPoints.Length];
        SuspensionDebugRays = new Ray[SuspensionPoints.Length];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // We handle physics here
    void FixedUpdate()
    {

        for( int i = 0; i < SuspensionPoints.Length; i++)
        {
            GameObject suspensionPt = SuspensionPoints[i];
            Vector3 direction = - transform.TransformDirection(suspensionPt.transform.up);
            Ray ray = new Ray(suspensionPt.transform.position, direction);
            
            RaycastHit hit;
            bool didWeHit = Physics.Raycast(ray, out hit, SuspensionDistance);

            // For debug purposes
            SuspensionDebugRays[i] = ray;
            SuspensionDebugHit[i] = hit;
     
            if (didWeHit)
            {
                Debug.Log(hit.collider.name);
            }

        }

    }

    void OnDrawGizmos()
    {

        if (SuspensionDebugHit == null)
            return;

        for (int i = 0; i < SuspensionPoints.Length; i++)
        {

            Ray ray = SuspensionDebugRays[i];
            RaycastHit hit = SuspensionDebugHit[i];

            Gizmos.DrawRay(ray.origin, ray.direction * SuspensionDistance);
            Gizmos.DrawSphere(hit.point, 0.1f);

        }

    }

}
