using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrWheel : MonoBehaviour
{
    
    
    private Rigidbody rigidBody;


    public bool wFL;
    public bool wFR;
    public bool wRL;
    public bool wRR;

    public float forwardForce = 45000;
    
    [Header("Suspension")] 
    public float resetLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    public float minLength;
    public float maxLength;
    public float lastLength;
    public float springLength;
    public float springVelocity;
    public float springForce;
    public float damperForce;

    private Vector3 suspensionForce;

    [Header("Wheel")] 
    public float wheelRadius;
    public float steerAngle;
    public float steerTime;
    
    private float wheelAngle;
    private Vector3 wheelVelocityLS;
    private float Fx;
    private float Fy;
    void Start()
    {
        rigidBody = transform.root.GetComponent<Rigidbody>();

        minLength = resetLength - springTravel;
        maxLength = resetLength + springTravel;
        
    }

    
    // private void FixedUpdate()
    // {
    //     minLength = resetLength - springTravel;
    //     maxLength = resetLength + springTravel;
    //     if (Physics.Raycast(transform.position,-transform.up , out RaycastHit hit , maxLength+wheelRadius))
    //     {
    //         lastLength = springLength;
    //         springLength = hit.distance - wheelRadius;
    //         springLength = Mathf.Clamp(springLength, minLength, maxLength);
    //         springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
    //         springForce = springStiffness * (resetLength - springLength);
    //         damperForce = damperStiffness * springVelocity;
    //
    //         springForce = 100;
    //         print(springLength );
    //         wheelVelocityLS = transform.InverseTransformDirection(rigidBody.GetPointVelocity(hit.point));
    //         Fx = Input.GetAxis("Vertical") * springForce;
    //         Fy = wheelVelocityLS.x * springForce;
    //
    //         suspensionForce = (springForce+damperForce) * transform.up;
    //         suspensionForce = 0* transform.up;
    //         rigidBody.AddForceAtPosition(suspensionForce  + (Fx * transform.forward) + (Fy * -transform.right),hit.point);
    //     }
    // }
    private void FixedUpdate()
    {
       
        if (Physics.Raycast(transform.position,-transform.up , out RaycastHit hit ))
        {
            wheelVelocityLS = transform.InverseTransformDirection(rigidBody.GetPointVelocity(hit.point));
            Fx = 0.4f * forwardForce;
            print(Fx);
            Fy = wheelVelocityLS.x * springForce;
            print(Fx + "| " + Fy);
            rigidBody.AddForceAtPosition(   (Fx * transform.forward) + (Fy * -transform.right),hit.point);
        }
    }


    void Update()
    {
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);
    }
}
