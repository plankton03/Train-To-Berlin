using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCenterOfMass : MonoBehaviour
{
    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.centerOfMass = new Vector3(0.001275f, 0f, -0.08944f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
