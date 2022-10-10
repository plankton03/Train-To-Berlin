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

public class CarController2 : MonoBehaviour
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


    public bool isBraking;

    public float brakingPower;

    public float limitBrakingPower;


    public float angle;

    public float turningBrakingPower;

    public float straightBrakingPower;

    public Vector3 velocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centerOfMass;
    }


    void Update()
    {
        GetInputs();
        HandleMovement();
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


    private void HandleMovement()
    {
        velocity = transform.InverseTransformDirection(rigidbody.velocity);
        if (velocity.z > 5 && verticalInput < 0)
        {
            isBraking = true;
            brakingPower = straightBrakingPower;
        }
        else if (velocity.z < -5 && verticalInput > 0)
        {
            isBraking = true;
            brakingPower = straightBrakingPower;
        }
        else if (Mathf.Abs(angle) > 5)
        {
            isBraking = true;
            brakingPower = turningBrakingPower;
        }
        else
        {
            isBraking = false;
            brakingPower = 0;
        }
    }

    private void Move()
    {
        if (isBraking)
        {
            foreach (var wheel in wheels)
            {
                wheel.collider.motorTorque = 0;
                wheel.collider.brakeTorque = brakingPower;
            }
        }
        else
        {
            mySpeed = rigidbody.velocity.magnitude;
            if (mySpeed < maxSpeed)
            {
                foreach (var wheel in wheels)
                {
                    wheel.collider.motorTorque = verticalInput * maxAcceleration * motorPower * Time.deltaTime;
                    wheel.collider.brakeTorque = brakingPower;
                }
            }
            else if (mySpeed > 1.1 * maxSpeed)
            {
                foreach (var wheel in wheels)
                {
                    wheel.collider.motorTorque = 0;
                    wheel.collider.brakeTorque = limitBrakingPower;
                    isBraking = true;
                }
            }
            else
            {
                foreach (var wheel in wheels)
                {
                    wheel.collider.motorTorque = 0;
                    wheel.collider.brakeTorque = brakingPower;
                }
            }
        }
    }

    private void Turn()
    {
        angle = horizontalInput * maxSteerAngle * turnSensitivity;
        if (Mathf.Abs(angle) <= Mathf.Epsilon)
        {
            rigidbody.velocity = new Vector3(0, 0, rigidbody.velocity.z);
        }
        else
        {
            foreach (Wheel wheel in wheels)
            {
                if (wheel.axel == Axel.Front)
                {
                    wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, angle, 0.5f);
                }
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