using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSuspension : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearRight;
    public bool wheelRearLeft;

    public enum TiltMode
    {
        Off,
        Right,
        Left
        
    }

    public TiltMode tiltMode;
    

    [Header("Suspension")] 
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingStiffness;

    private float _minLength;
    private float _maxLength;
    private float _springLength;
    private float _lastSpringLength;
    
    private float _springForce;
    private float _dampingForce;
    private float _springVelocity;
    private Vector3 _suspensionForce;

    private Vector3 _forwardForce;
    private Vector3 _rightForce;


    private float wheelPos;
    

    [Header("Wheel")] 
    public float wheelRadius;
    public float steerAngle;
    public float steerTime;
    public float wheelSpeed;
    public Transform wheelMesh;

    private float _wheelCurrentAngle;
    private Vector3 _wheelVelocityLocalSpace;
    void Start()
    {
        _rigidbody = transform.root.GetComponent<Rigidbody>();

        _minLength = restLength - springTravel;
        _maxLength = restLength + springTravel;
    }

    private void Update()
    {
        RotateYAxis();

        Debug.DrawRay(transform.position , -transform.up * (_springLength + wheelRadius) , Color.magenta);
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position , -transform.up , out RaycastHit hit , _maxLength + wheelRadius))
        {
            _lastSpringLength = _springLength;
            _springLength = hit.distance - wheelRadius;
            _springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);
            
            _springVelocity = (_lastSpringLength - _springLength)/Time.fixedDeltaTime;

            _springForce = springStiffness * (restLength - _springLength);
            _dampingForce = dampingStiffness * _springVelocity;
            _suspensionForce = (_springForce+_dampingForce)  * transform.up;

            _wheelVelocityLocalSpace = transform.InverseTransformDirection(_rigidbody.GetPointVelocity(hit.point));
            _forwardForce = (Input.GetAxis("Vertical") * wheelSpeed) * transform.forward;
            _rightForce = (_wheelVelocityLocalSpace.x * wheelSpeed )* -transform.right;
            
            
            
            
            // _rigidbody.AddForceAtPosition(_suspensionForce +_forwardForce + _rightForce ,hit.point);
            print(_springLength);
            _rigidbody.AddForceAtPosition(_suspensionForce ,hit.point);
            
            SetWheelMeshPos();
            
        }
        else
        {
            _springLength = _maxLength;
            print("Fuck");
        }
        
        
    }

    private void SetWheelMeshPos()
    {
        float finalPos;
        if (tiltMode == TiltMode.Right)
        {
            if (wheelFrontRight || wheelRearRight)
            {
                
                wheelMesh.localPosition = new Vector3(0, -_maxLength, 0);
            }
            else
            {
                wheelMesh.localPosition = new Vector3(0, -_minLength, 0);
            }
        }else if (tiltMode == TiltMode.Left)
        {
            if (wheelFrontRight || wheelRearRight)
            {
                wheelMesh.localPosition = new Vector3(0, -_minLength, 0);
            }
            else
            {
                wheelMesh.localPosition = new Vector3(0, -_maxLength, 0);
            }
        }
        else
        {
            wheelMesh.localPosition = new Vector3(0, -_springLength, 0);
        }
    }

    private void RotateYAxis()
    {
        _wheelCurrentAngle = Mathf.Lerp(_wheelCurrentAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * _wheelCurrentAngle);
    }
}
