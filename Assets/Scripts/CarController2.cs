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



//
//
//
// void Update()
//     {
//         CheckInput();
//
//         CheckIsBlocked();
//         // UpdateWheelTransform();
//         // Tilt();
//     }
//
//     private void CheckIsBlocked()
//     {
//         if (Physics.BoxCast(frontBlockedCheckerPoint.position , new Vector3(0.5f , 0.45f , 0f) ,
//                 transform.forward , Quaternion.identity , 0.2f))
//         {
//             Debug.DrawRay(frontBlockedCheckerPoint.position , transform.forward , Color.red , 0.4f);
//             isFrontBlocked = true;
//
//         }
//         else
//         {
//             isFrontBlocked = false;
//
//         }
//         if (Physics.BoxCast(rearBlockedCheckerPoint.position , new Vector3(0.5f , 0.45f , 0f) ,
//                 -transform.forward , Quaternion.identity , 0.3f))
//         {
//             Debug.DrawRay(rearBlockedCheckerPoint.position , -transform.forward , Color.red , 0.4f);
//             isRearBlocked = true;
//
//         }
//         else
//         {
//             isRearBlocked = false;
//
//         }
//         
//     }
//
//     private void FixedUpdate()
//     {
//
//         if (isOnGround() && !isBlocked())
//         {
//             MoveForward();
//         
//             Turn();   
//         }
//
//         if (isOnGround() && isBlocked() )
//         {
//             // _rigidbody.velocity = transform.TransformDirection(new Vector3(0, 0, 0));
//             print("No movement");
//             _rigidbody.velocity = Vector3.zero;
//             forwardVelocity = 0;
//
//         }
//
//             
//             Fall();
//         // _rigidbody.AddTorque(Vector3.up * steeringSpeed * Time.fixedDeltaTime * _rigidbody.mass * steerInput);
//     }
//
//     private bool isBlocked()
//     {
//         if (verticalInput <-0.1 && isRearBlocked)
//         {
//             print("Blocked");
//             return true;
//         }
//
//         if (verticalInput>=-0.1 && isFrontBlocked)
//         {
//             print("Blocked");
//             return true;
//         }
//
//         return false;
//     }
//     private void UpdateWheelTransform()
//     {
//         CalculateSteerAngles();
//         SetSteerAngles();
//     }
//
//     private void CheckInput()
//     {
//         steerInput = Input.GetAxis("Horizontal");
//         verticalInput = Input.GetAxis("Vertical");
//     }
//
//     private bool isOnGround()
//     {
//         int numOgWheelsOnGround = 0; 
//         foreach (WheelSuspension wheel in wheels)
//         {
//             if (wheel.isOnGround)
//             {
//                 numOgWheelsOnGround++;
//             }
//         }
//
//         if (numOgWheelsOnGround >= 2)
//         {
//             return true;
//         }
//
//         return false;
//     }
//
//     private void MoveForward()
//     {
//         // _rigidbody.AddRelativeForce(new Vector3(Vector3.forward.x,0,Vector3.forward.z) * speed * verticalInput * Time.fixedDeltaTime * _rigidbody.mass);
//         // Vector3 localVelocity =transform.InverseTransformDirection( _rigidbody.velocity);
//         // localVelocity.x = 0;
//         // _rigidbody.velocity = transform.TransformDirection(localVelocity);
//         float finalSpeed = (verticalInput < 0) ? maxSpeed * -0.8f : maxSpeed;
//         forwardVelocity = Mathf.Lerp(forwardVelocity, finalSpeed, accelTime * Time.fixedDeltaTime);
//         
//         localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
//         localVelocity.x = 0;
//         localVelocity.z = forwardVelocity;
//
//         _rigidbody.velocity = transform.TransformDirection(localVelocity);
//     }
//
//     // private void FixedUpdate()
//     // {
//     //     // var localRotation = transform.localRotation;
//     //     // transform.localRotation  = Quaternion.Euler(localRotation.x,localRotation.y + steerInput*30 , localRotation.z);
//     //     Vector3 vel = new Vector3(0, 0, speed * verticalInput);
//     //     
//     //     Turn();
//     //     
//     //     Vector3 rbVel = transform.InverseTransformDirection(_rigidbody.velocity);
//     //     
//     //     Vector3 ws = transform.TransformVector(vel);
//     //     print("WS  = " + ws);
//     //     print("LS  =" + rbVel);
//     //
//     //     // _rigidbody.velocity = ws;
//     // }
//
//     private void CalculateSteerAngles()
//     {
//         if (steerInput > 0)
//         {
//             _ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
//             _ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
//         }
//         else if (steerInput < 0)
//         {
//             _ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
//             _ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
//         }
//         else
//         {
//             _ackermannAngleLeft = 0f;
//             _ackermannAngleRight = 0f;
//         }
//     }
//
//     private void SetSteerAngles()
//     {
//         foreach (WheelSuspension wheel in wheels)
//         {
//             if (wheel.wheelFrontLeft)
//             {
//                 wheel.steerAngle = _ackermannAngleLeft;
//             }
//             else if (wheel.wheelFrontRight)
//             {
//                 wheel.steerAngle = _ackermannAngleRight;
//             }
//         }
//     }
//
//     private void Turn()
//     {
//         float angle = Mathf.Rad2Deg * (steeringSpeed * steerInput * Time.fixedDeltaTime);
//
//         Vector3 transformEulerAngles = transform.eulerAngles;
//         transform.rotation =
//             Quaternion.Euler(transformEulerAngles.x, transformEulerAngles.y + angle, transformEulerAngles.z);
//     }
//
//     private void Tilt()
//     {
//         _tiltAngle = Mathf.Lerp(_tiltAngle, steerInput * tiltModifier * Mathf.Rad2Deg,
//             Time.deltaTime * tiltTime);
//         
//         Vector3 transformEulerAngles = bodyMesh.eulerAngles;
//         bodyMesh.rotation = Quaternion.Euler(transformEulerAngles.x, transformEulerAngles.y, _tiltAngle);
//         // bodyMesh.rotation = Quaternion.Euler(0, 0, _tiltAngle);
//     }
//
//     private void Fall()
//     {
//         _rigidbody.AddForce(Vector3.down * _rigidbody.mass * gravityMultiplier);
//     }
// }