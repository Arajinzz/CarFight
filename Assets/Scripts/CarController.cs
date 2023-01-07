using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public GameObject[] SuspensionPoints;
    public float SuspensionDistance = 0.5f;
    public float SuspensionStrength = 100000.0f;
    public float SuspensionDamping = 100;

    private Ray[] SuspensionDebugRays;
    private RaycastHit[] SuspensionDebugHit;

    private Rigidbody carRb;


    // Start is called before the first frame update
    void Start()
    {

        SuspensionDebugHit = new RaycastHit[SuspensionPoints.Length];
        SuspensionDebugRays = new Ray[SuspensionPoints.Length];

        carRb = GetComponent<Rigidbody>();

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
            Vector3 direction = transform.TransformDirection(suspensionPt.transform.up);
            Ray ray = new Ray(suspensionPt.transform.position, -direction);
            
            RaycastHit hit;
            bool didWeHit = Physics.Raycast(ray, out hit, SuspensionDistance);

            // For debug purposes
            SuspensionDebugRays[i] = ray;
            SuspensionDebugHit[i] = hit;
     
            if (didWeHit)
            {
                // Calculate compression ratio
                float compression = 1 - (hit.distance / SuspensionDistance);
                // Calculate force to be applied
                float force = (compression * SuspensionStrength) - (SuspensionDamping * carRb.GetPointVelocity(suspensionPt.transform.position).y);
                // Calculate force, (Opposite of raycast)
                carRb.AddForceAtPosition(direction * force, suspensionPt.transform.position);
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
