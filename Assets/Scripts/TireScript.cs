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

    // Car rigidbody
    private Rigidbody carRigidbody = null;

    void Start()
    {
        carRigidbody = this.GetComponentInParent<Rigidbody>();

        springStrength = 6000.0f;
        springDamper = 100.0f;
        wheelRadius = 0.8f;
        springRestDistance = wheelRadius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        RaycastHit hit;

        Debug.DrawRay(transform.position, -transform.up * wheelRadius, Color.green);

        if (Physics.Raycast(transform.position, -transform.up, out hit, wheelRadius))
        {

            Vector3 springDirection = transform.up;

            Vector3 tireVelocity = carRigidbody.GetPointVelocity(transform.position);

            float offset = springRestDistance - hit.distance;

            float velocity = Vector3.Dot(springDirection, tireVelocity);

            float force = (offset * springStrength) - (velocity * springDamper);

            carRigidbody.AddForceAtPosition(springDirection * force, transform.position);

        }

    }
}
