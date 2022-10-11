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
    public float steeringSpeed;
    public float maxSpeed;
    public float accelTime;

    [Header("Inputs")]
    public float steerInput;
    public float verticalInput;

    public Vector3 localVelocity;

    public float _ackermannAngleLeft;
    public float _ackermannAngleRight;


    private float forwardVelocity;
    
    
    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        steerInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        CalculateSteerAngles();
        SetSteerAngles();

    }

    private void FixedUpdate()
    {
        MoveForward();
        
        Turn();   
        // _rigidbody.AddTorque(Vector3.up * steeringSpeed * Time.fixedDeltaTime * _rigidbody.mass * steerInput);
    }

    private void MoveForward()
    {
        // _rigidbody.AddRelativeForce(new Vector3(Vector3.forward.x,0,Vector3.forward.z) * speed * verticalInput * Time.fixedDeltaTime * _rigidbody.mass);
        // Vector3 localVelocity =transform.InverseTransformDirection( _rigidbody.velocity);
        // localVelocity.x = 0;
        // _rigidbody.velocity = transform.TransformDirection(localVelocity);
        float finalSpeed = (verticalInput < 0) ? maxSpeed * -0.8f : maxSpeed;
        forwardVelocity = Mathf.Lerp(forwardVelocity, finalSpeed, accelTime * Time.fixedDeltaTime);
        localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
        localVelocity.x = 0;
        localVelocity.z = forwardVelocity;

        _rigidbody.velocity = transform.TransformDirection(localVelocity);

    }

    // private void FixedUpdate()
    // {
    //     // var localRotation = transform.localRotation;
    //     // transform.localRotation  = Quaternion.Euler(localRotation.x,localRotation.y + steerInput*30 , localRotation.z);
    //     Vector3 vel = new Vector3(0, 0, speed * verticalInput);
    //     
    //     Turn();
    //     
    //     Vector3 rbVel = transform.InverseTransformDirection(_rigidbody.velocity);
    //     
    //     Vector3 ws = transform.TransformVector(vel);
    //     print("WS  = " + ws);
    //     print("LS  =" + rbVel);
    //
    //     // _rigidbody.velocity = ws;
    // }

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
    
    private void Turn()
    {
        float angle = Mathf.Rad2Deg * (steeringSpeed * steerInput * Time.deltaTime);

        Vector3 transformEulerAngles = transform.eulerAngles;
        transform.rotation =
            Quaternion.Euler(transformEulerAngles.x, transformEulerAngles.y + angle, transformEulerAngles.z);
    }
}
