using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;


    [Header("User Interface")] [Range(0, 0.5f)]
    public float turnTouchArea = 0.5f;

    [Header("General")] public float maxSteeringAngle = 30f;
    public float motorForce = 50f;
    public float brakeForce = 3000f;
    [Range(0, 1f)] public float backwardSpeedPercentage = 0.8f;

    [Header("Colliders")] public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider backLeftWheelCollider;
    public WheelCollider backRightWheelCollider;

    [Header("Meshes")] public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform backLeftWheelTransform;
    public Transform backRightWheelTransform;

    private void FixedUpdate()
    {
        GetInputPC();
        HandleMotor();
        HandleSteering();
        UpdateWheelsVisuals();
    }

    private void GetInputCellphone()
    {
        if (Input.touches.Length >= 2)
            verticalInput = -1 * backwardSpeedPercentage;

        else if (Input.touches.Length > 0)
        {
            //todo : x or y ????
            if (Input.GetTouch(0).position.x < Screen.width * turnTouchArea)
                horizontalInput = -1f;
            else if (Input.GetTouch(0).position.x > Screen.width * (1 - turnTouchArea))
                horizontalInput = +1f;
        }
    }

    //for test
    private void GetInputPC()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;

        float currBrakeForce = isBreaking ? brakeForce : 0f;
        frontLeftWheelCollider.brakeTorque = currBrakeForce;
        frontRightWheelCollider.brakeTorque = currBrakeForce;
        backLeftWheelCollider.brakeTorque = currBrakeForce;
        backRightWheelCollider.brakeTorque = currBrakeForce;
    }

    private void HandleSteering()
    {
        steerAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }

    private void UpdateWheelsVisuals()
    {
        UpdateWheelPos(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPos(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPos(backLeftWheelCollider, backLeftWheelTransform);
        UpdateWheelPos(backRightWheelCollider, backRightWheelTransform);
    }

    private void UpdateWheelPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }
}