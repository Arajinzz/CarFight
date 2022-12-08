using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyShpere : MonoBehaviour
{
    [SerializeField]
    float force = 100.0f;

    // Start is called before the first frame update
    void Start()
    {

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, 0, rb.mass * force));
        
    }
}
