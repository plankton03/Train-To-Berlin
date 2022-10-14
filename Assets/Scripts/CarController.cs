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

    public BoxCollider frontBoxCollider;
    public BoxCollider rearBoxCollider;

    public Transform frontBlockedCheckerPoint;
    public Transform rearBlockedCheckerPoint;



    [Header("Car specs")] public float wheelBase;
    public float rearTrack;
    public float turnRadius;
    public float speed;
    public float steeringSpeed;
    public float maxSpeed;
    public float accelTime;

    [Header("Inputs")] public float steerInput;
    public float verticalInput;

    [Header("bara check")] public Vector3 localVelocity;
    public float _ackermannAngleLeft;
    public float _ackermannAngleRight;
    public bool isFrontBlocked;
    public bool isRearBlocked;


    [Header("Visual")] public float tiltModifier;
    public float tiltTime;

    private float _tiltAngle;


    public float gravityMultiplier;
    private float forwardVelocity;

    public Transform bodyMesh;


    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckInput();
        UpdateWheelTransform();

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

     private void CheckInput()
     {
         steerInput = Input.GetAxis("Horizontal");
         verticalInput = Input.GetAxis("Vertical");
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