using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Axel
{
    Front,
    Rear
}

[Serializable]
public struct Wheel
{
    public Transform model;
    public WheelCollider collider;
    public Axel axel;
}

public class CarController : MonoBehaviour
{
    [SerializeField] private List<Wheel> wheels;
    public float horizontalInput;
    public float verticalInput;
    public float maxSpeed;
    public float maxSteerAngle;
    public float maxAcceleration;
    public float motorPower;
    public float turnSensitivity;
    public float mySpeed;
    public Vector3 centerOfMass;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centerOfMass;
    }


    void Update()
    {
        GetInputs();
        Move();
        Turn();
    }



    private void LateUpdate()
    {
        AnimateWheels();
    }

    private void GetInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Vertical") < 0)
            verticalInput = -1;
        else
            verticalInput = 1;
    }
    


    private void Move()
    {
        mySpeed = rigidbody.velocity.magnitude;
        if (mySpeed < maxSpeed)
        {
            foreach (var wheel in wheels)
            {
                wheel.collider.motorTorque = verticalInput * maxAcceleration * motorPower * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.collider.motorTorque = 0;
            }
        }
    }

    private void Turn()
    {
        foreach (Wheel wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                float angle = horizontalInput * maxSteerAngle * turnSensitivity ;
                print(angle);
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, angle, 0.5f);
            }
        }
    }

    private void AnimateWheels()
    {
        foreach (Wheel wheel in wheels)
        {
            Quaternion wheelRotation;
            Vector3 wheelPosition;
            wheel.collider.GetWorldPose(out wheelPosition, out wheelRotation);
            wheel.model.transform.position = wheelPosition;
            wheel.model.transform.rotation = wheelRotation;
        }
    }
}