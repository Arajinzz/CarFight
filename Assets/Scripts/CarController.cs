using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    private struct SuspensionParams
    {
        public float compRatio;
        public Vector3 impactPt;
        public Vector3 impactNormal;
    }
    
    private Rigidbody carRb;

    public GameObject[] SuspensionPoints;
    public float SuspensionDistance = 0.5f;
    public float SuspensionStrength = 100.0f;
    public float SuspensionDamping = 2;
    private SuspensionParams[] SuspensionCache;

    private Ray[] SuspensionDebugRays;
    private RaycastHit[] SuspensionDebugHit;

    public float AccelPower = 100.0f;

    public float TorquePower = 50.0f;

    public float Traction = 10.0f; // Slip reduction

    // Input state
    bool isAccel = false;
    bool isBrake = false;
    bool isTurningRight = false;
    bool isTurningLeft = false;

    // Start is called before the first frame update
    void Start()
    {

        SuspensionDebugHit = new RaycastHit[SuspensionPoints.Length];
        SuspensionDebugRays = new Ray[SuspensionPoints.Length];

        SuspensionCache = new SuspensionParams[SuspensionPoints.Length];

        carRb = GetComponent<Rigidbody>();

        // Extra protection
        isAccel = false;
        isBrake = false;
        isTurningRight = false;
        isTurningLeft = false;

    }

    // Update is called once per frame
    void Update()
    {
        isAccel = Input.GetKey(KeyCode.W);
        isBrake = Input.GetKey(KeyCode.S);
        isTurningLeft = Input.GetKey(KeyCode.A);
        isTurningRight = Input.GetKey(KeyCode.D);

        // If we're accelerating override Brake (don't brake)
        // This is just to test smoothly, it will be changed
        isBrake = isAccel ? false : isBrake;

        // Override
        isTurningLeft = isTurningRight ? false : isTurningLeft;
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

                // Store in cache
                SuspensionCache[i].compRatio = compression;
                SuspensionCache[i].impactPt = hit.point;
                SuspensionCache[i].impactNormal = hit.normal;
            }

        }

        // TODO:
        // ACCELERATION NEEDS TO BE PROJECTED TO GROUND TO PREVENT FLYING
        if (isAccel)
        {
            // Will just apply some force here
            // But i think it needs more work, because it takes time for a vehicle to reach max speed
            // So i think it needs a rework (a way to properly simulate acceleration)
            carRb.AddForceAtPosition(transform.forward * AccelPower, transform.position); // Center of mass is center of vehicle CURRENTLY
        }

        if (isBrake)
        {
            carRb.AddForceAtPosition(-transform.forward * AccelPower, transform.position); // Center of mass is center of vehicle CURRENTLY
        }

        if (isTurningLeft)
        {
            carRb.AddTorque(-transform.up * TorquePower);
        }

        if (isTurningRight)
        {
            carRb.AddTorque(transform.up * TorquePower);
        }

        // NOT FINISHED, MAYBE WILL BE REPLACED BY ANGULAR DRAG IN RIGIDBODY

        // Adding Traction / Slip Reduction
        // To prevent the car from sliding
        Vector3 carVelocity = carRb.GetPointVelocity(transform.position);
        // Positive means force to right
        float sidewayVelocity = Vector3.Dot(transform.right, carVelocity);

        float velocityChange = -sidewayVelocity * Traction;
        float temp = velocityChange / Time.fixedDeltaTime;

        Vector3 tempDir = (carVelocity - transform.position).normalized;

        Debug.DrawRay(transform.position, transform.right * Traction * -sidewayVelocity, Color.red);
        Debug.DrawRay(transform.position, transform.forward * Traction, Color.blue);
        carRb.AddForceAtPosition(transform.right * Traction * sidewayVelocity, transform.position);

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
