using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class CarMovementHandler : MonoBehaviour
{
    [Header("Input values")] public float horizontalInput;
    public float verticalInput;

    [Header("Forward movement variables")] public float moveFactor = 30;

    [Header("Turning variables")] public float turnFactor;


    [Header("Visual")] public float tiltModifier;
    public float tiltTime;


    private float _tiltAngle;


    private int _numOfGroundTagObjects = 0;
    private Rigidbody _rigidbody;

    public Transform centerOfMass;

    private Vector3 previousLoc;

    private bool flag = true;

    public Vector3 velocity;

    private bool _moveable = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // _rigidbody.centerOfMass = centerOfMass.localPosition;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            _numOfGroundTagObjects++;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            _numOfGroundTagObjects--;
        }
    }

//     void Update()
//     {
//         if (_numOfGroundTagObjects > 0)
//         {
//             previousLoc = transform.position;
//             CheckInput();
//             MoveForward();
//             Turn();
//             Visual();
//             
//             flag = false;
//         }
//         else
//         {
//             
//             if (!flag)
//             {
//                 velocity = (transform.position - previousLoc) / Time.deltaTime;
//                 _rigidbody.velocity = velocity;
//                 flag = true;
//             }
// previousLoc = transform.position;
//             
//
//         }
//     }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.transform.tag.Equals("Obstacle"))
        {
            _moveable = false;
            print("can't move");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.tag.Equals("Obstacle"))
        {
            _moveable = true;
            print("movement is ok");
        }
    }

    void Update()
    {
        CheckInput();
        MoveForward();
        if (_numOfGroundTagObjects > 0)
        {
            Turn();
            Visual();
        }
    }

    private void CheckInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = 0.5f;
    }

    private void MoveForward()
    {
        if (_moveable)
        {
            transform.position += (verticalInput * Time.deltaTime * transform.forward) * moveFactor;
   
        }
    }

    private void Turn()
    {
        float angle = Mathf.Rad2Deg * (turnFactor * horizontalInput * Time.deltaTime);

        Vector3 transformEulerAngles = transform.eulerAngles;
        transform.rotation =
            Quaternion.Euler(transformEulerAngles.x, transformEulerAngles.y + angle, transformEulerAngles.z);
    }

    private void Visual()
    {
        _tiltAngle = Mathf.Lerp(_tiltAngle, turnFactor * horizontalInput * tiltModifier * Mathf.Rad2Deg,
            tiltTime * Time.deltaTime);


        Vector3 transformEulerAngles = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(transformEulerAngles.x, transformEulerAngles.y, _tiltAngle);
    }
}