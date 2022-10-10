using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class CarController : MonoBehaviour
{
    public WheelSuspension[] wheels;
    private Rigidbody _rigidbody;

    [Header("Car specs")] 
    public float wheelBase;
    public float rearTrack;
    public float turnRadius;
    public float speed;
    public Vector3 velocity;

    [Header("Inputs")]
    public float steerInput;

    public float _ackermannAngleLeft;
    public float _ackermannAngleRight;
    
    
    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        steerInput = Input.GetAxis("Horizontal");

        CalculateSteerAngles();
        SetSteerAngles();

    }

    private void FixedUpdate()
    {
        Vector3 vel = new Vector3(0, 0, speed);
        
        _rigidbody.velocity = vel;
    }

    private void CalculateSteerAngles()
    {
        if (steerInput > 0)
        {
            _ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            _ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;

        }else if (steerInput < 0)
        {
            _ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
            _ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
        }
        else
        {
            _ackermannAngleLeft = 0f;
            _ackermannAngleRight = 0f;
        }
    }

    private void SetSteerAngles()
    {
        foreach (WheelSuspension wheel in wheels)
        {
            if (wheel.wheelFrontLeft)
            {
                wheel.steerAngle = _ackermannAngleLeft;
            }else if (wheel.wheelFrontRight)
            {
                wheel.steerAngle = _ackermannAngleRight;
            }
        }
    }
}
