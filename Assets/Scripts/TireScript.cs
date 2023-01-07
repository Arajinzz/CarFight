using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireScript : MonoBehaviour
{

    [SerializeField]
    float springStrength = 500.0f;

    [SerializeField]
    float springDamper = 10.0f;

    [SerializeField]
    float wheelRadius = 0.5f;

    [SerializeField]
    float springRestDistance = 1.0f;

    [SerializeField]
    float tireGripFactor = 1.0f; /* From 0 to 1 */

    [SerializeField]
    float tireMass = 10.0f;

    [SerializeField]
    float accelInput = 0.0f;

    [SerializeField]
    AnimationCurve powerCurve;

    [SerializeField]
    float carTopSpeed = 100.0f; // This should go to a CAR script it is illogical to have car speed on tires

    // Car rigidbody
    private Rigidbody carRigidbody = null;

    void Start()
    {
        carRigidbody = this.GetComponentInParent<Rigidbody>();

        /* Test values */

        springStrength = 2000.0f;
        springDamper = 200.0f;
        wheelRadius = 1f;
        springRestDistance = wheelRadius;

        tireGripFactor = 0.8f;
        tireMass = 5f;

        accelInput = 0.0f;
        carTopSpeed = 100000.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            accelInput = 2500.0f;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            accelInput = 0;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            accelInput = -2500.0f;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            accelInput = 0;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 50, 0));
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, -50, 0));
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }


    void FixedUpdate()
    {

        RaycastHit hit;

        Debug.DrawRay(transform.position, -transform.up * wheelRadius, Color.green);

        // Suspension Force
        if (Physics.Raycast(transform.position, -transform.up, out hit, wheelRadius))
        {

            Vector3 springDirection = transform.up;

            Vector3 tireVelocity = carRigidbody.GetPointVelocity(transform.position);

            float offset = springRestDistance - hit.distance;

            float velocity = Vector3.Dot(springDirection, tireVelocity);

            float force = (offset * springStrength) - (velocity * springDamper);

            carRigidbody.AddForceAtPosition(springDirection * force, transform.position);

        }

        // Steering Force
        if (Physics.Raycast(transform.position, -transform.up, out hit, wheelRadius))
        {

            Vector3 steeringDirection = transform.right;

            Vector3 tireVelocity = carRigidbody.GetPointVelocity(transform.position);

            float steeringVelocity = Vector3.Dot(steeringDirection, tireVelocity);

            float desiredVelocityChange = -steeringVelocity * tireGripFactor;

            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            carRigidbody.AddForceAtPosition(steeringDirection * tireMass * desiredAcceleration, transform.position);

        }

        // Acceleration / Braking Force
        if (Physics.Raycast(transform.position, -transform.up, out hit, wheelRadius))
        {

            Vector3 accelDirection = transform.right; /* Should be forward but my Car axis is messed up*/

            // Acceleration torque
            if (accelInput > 0.0f)
            {

                float carSpeed = Vector3.Dot(carRigidbody.transform.forward, carRigidbody.velocity);

                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

                float availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelInput;

                carRigidbody.AddForceAtPosition(accelDirection * availableTorque, transform.position);

            }


        }

    }
}
