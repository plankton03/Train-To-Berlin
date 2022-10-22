using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class CarController : MonoBehaviour
{
    public WheelSuspension[] wheels;

    [Header("Car specs")] public float wheelBase;
    public float rearTrack;
    public float turnRadius;
    public float maxSpeed;

    [Header("Inputs")] 
    public float steerInput;
    public float verticalInput;

    private float _ackermannAngleLeft;
    private float _ackermannAngleRight;
    

    private void Update()
    { 
        UpdateWheelTransform();
        UpdateWheelInfo();
    }

    private void UpdateWheelInfo()
    {
        foreach (WheelSuspension wheel in wheels)
        {
            wheel.accelInput = verticalInput;
            wheel.carTopSpeed = maxSpeed;
        }
    }

    private void UpdateWheelTransform()
    {
        CalculateSteerAngles();
        SetSteerAngles();
    }


    private void CalculateSteerAngles()
    {
        if (steerInput > 0)
        {
            _ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            _ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
        }
        else if (steerInput < 0)
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
            }
            else if (wheel.wheelFrontRight)
            {
                wheel.steerAngle = _ackermannAngleRight;
            }
        }
    }
}