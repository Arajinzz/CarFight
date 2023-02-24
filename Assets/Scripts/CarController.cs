using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarController : MonoBehaviour
{

    [SerializeField]
    CinemachineVirtualCamera FollowCamera;

    private struct SuspensionParams
    {
        public float compRatio;
        public Vector3 impactPt;
        public Vector3 impactNormal;
    }
    
    private Rigidbody carRb;

    public GameObject[] SuspensionPoints;
    private Transform[] WheelsMeshes;
    private bool WheelsDisable = true;

    public float SuspensionDistance = 0.5f;
    public float SuspensionStrength = 100.0f;
    public float SuspensionDamping = 2;
    private SuspensionParams[] SuspensionCache;

    private Ray[] SuspensionDebugRays;
    private RaycastHit[] SuspensionDebugHit;

    public float AccelPower = 5.0f;
    public float TopCarForce = 100.0f;
    private float CarCurrentForce;
    
    private bool oneWheelInGround = false;

    public float TorquePower = 50.0f;

    // Falling
    public float drag = 2.5f;

    // Input state
    bool isAccel = false;
    bool isBrake = false;
    bool isTurningRight = false;
    bool isTurningLeft = false;


    // Shooting params
    public float ShootingRate = 0.5f;
    public GameObject ShootingPoint;
    private float ShootTimer = 0.0f;
    public GameObject Projectile;


    // Start is called before the first frame update
    void Start()
    {

        CarCurrentForce = 0.0f;

        SuspensionDebugHit = new RaycastHit[SuspensionPoints.Length];
        SuspensionDebugRays = new Ray[SuspensionPoints.Length];

        SuspensionCache = new SuspensionParams[SuspensionPoints.Length];

        WheelsMeshes = new Transform[SuspensionPoints.Length];
        // Get wheels
        for (int i = 0; i < SuspensionPoints.Length; i++)
        {
            // find the wheel
            foreach (Transform t in SuspensionPoints[i].transform)
            {
                if (t.tag.Equals("Wheel"))
                {
                    WheelsMeshes[i] = t;
                    // Set wheel position to the suspension distance
                    // Temporary value to be adjusted later (needs to calculated with wheel radius in mind)
                    t.position = new Vector3(t.position.x, SuspensionPoints[i].transform.position.y - (SuspensionDistance - 0.5f), t.position.z);
                }
            }
        }

        carRb = GetComponent<Rigidbody>();

        // Extra protection
        isAccel = false;
        isBrake = false;
        isTurningRight = false;
        isTurningLeft = false;

    }

    private void Update()
    {
        // Wheels fake mouvements
        for (int i = 0; i < WheelsMeshes.Length; i++)
        {
            Transform wheel = WheelsMeshes[i];
            Vector3 impactPoint = SuspensionCache[i].impactPt;
            Vector3 newPosition = new Vector3(wheel.position.x, SuspensionPoints[i].transform.position.y - ((1 - SuspensionCache[i].compRatio) * (SuspensionDistance - 0.5f)), wheel.position.z);
            wheel.position = Vector3.Lerp(wheel.position, newPosition, Time.deltaTime * 100f);
            wheel.gameObject.SetActive(!WheelsDisable);
        }
    }

    private void FixedUpdate()
    {
        oneWheelInGround = false;

        // Suspension
        for (int i = 0; i < SuspensionPoints.Length; i++)
        {
            GameObject suspensionPt = SuspensionPoints[i];
            Vector3 direction = transform.TransformDirection(suspensionPt.transform.up);
            Ray ray = new Ray(suspensionPt.transform.position, -direction);

            RaycastHit hit;
            bool didWeHit = Physics.Raycast(ray, out hit, SuspensionDistance);

            // For debug purposes
            SuspensionDebugRays[i] = ray;
            SuspensionDebugHit[i] = hit;

            // Calculate compression ratio
            float compression = 1 - (hit.distance / SuspensionDistance);

            if (didWeHit)
            {
                // Calculate force to be applied
                float force = (compression * SuspensionStrength) - (SuspensionDamping * carRb.GetPointVelocity(suspensionPt.transform.position).y);
                // Calculate force, (Opposite of raycast)
                carRb.AddForceAtPosition(direction * force, suspensionPt.transform.position);

                SuspensionCache[i].impactPt = hit.point;
                SuspensionCache[i].impactNormal = hit.normal;
            }

            // Store in cache
            SuspensionCache[i].compRatio = compression;

            oneWheelInGround |= didWeHit;

        }
    }

    public void ProcessMouvement(Structs.Inputs input, float deltaTime)
    {
        isAccel = input.up;
        isBrake = input.down;
        isTurningLeft = input.left;
        isTurningRight = input.right;
        carRb.drag = drag;

        // If we're accelerating override Brake (don't brake)
        // This is just to test smoothly, it will be changed
        isBrake = isAccel ? false : isBrake;

        // Override
        isTurningLeft = isTurningRight ? false : isTurningLeft;

        // Calculate projection of forward vector onto the ground
        // or wherever the car is running on
        Vector3 normal = SuspensionCache[0].impactNormal;
        Vector3 projForward = Vector3.ProjectOnPlane(transform.forward, normal);

        // TODO : ACCELERATE ONLY WHEN BACK WHEELS ARE HITING THE GROUND
        if (isAccel && oneWheelInGround)
        {
            CarCurrentForce = Mathf.MoveTowards(CarCurrentForce, TopCarForce, AccelPower * deltaTime);
            carRb.AddForceAtPosition(projForward * CarCurrentForce, transform.position); // Center of mass is center of vehicle CURRENTLY
        }

        if (isBrake && oneWheelInGround)
        {
            carRb.AddForceAtPosition(-projForward * AccelPower, transform.position); // Center of mass is center of vehicle CURRENTLY
        }

        if (isTurningLeft)
        {
            carRb.AddTorque(-transform.up * TorquePower);
        }

        if (isTurningRight)
        {
            carRb.AddTorque(transform.up * TorquePower);
        }

        if (!oneWheelInGround)
        {
            carRb.drag = 0;
        }


    }

    public void SetCamera()
    {
        // Bind virtual camera
        FollowCamera = GameObject.Find("VCam").GetComponent<CinemachineVirtualCamera>();
        FollowCamera.Follow = transform;
        FollowCamera.LookAt = transform;
    }

    public float Server_ProcessShooting(Structs.Inputs inputs, float deltaTime)
    {
        ShootTimer += deltaTime;

        // Shoot
        if (inputs.rclick && ShootTimer >= ShootingRate)
        {
            float return_value = ShootTimer;
            ShootTimer = 0.0f;

            // Instantiate Projectile
            Instantiate(Projectile, ShootingPoint.transform.position, ShootingPoint.transform.rotation);

            return return_value;
        }

        return ShootTimer;
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
