using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Serialization;

public class WheelSuspension : MonoBehaviour
{
    private Rigidbody _rigidbody;

    
    [Header("Wheel position")]
    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearRight;
    public bool wheelRearLeft;

    

    [Header("Suspension")] public float restLength;
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


    [Header("Wheel")] 
    public float wheelRadius;
    public float steerAngle;
    public float steerTime;
    public Transform wheelMesh;

    private float _wheelCurrentAngle;


    [Header("About car controlling")]
    public float accelInput;
    public float carTopSpeed;
    public AnimationCurve powerCurve;
    public AnimationCurve gripFactor;
    // [Range(0, 1)] public float tireGripFactor;
    public float wheelGripFactor;

    void Start()
    {
        _rigidbody = transform.root.GetComponent<Rigidbody>();

        _minLength = restLength - springTravel;
        _maxLength = restLength + springTravel;
    }

    private void Update()
    {
        WheelMeshVision();
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, _maxLength + wheelRadius))
        {
     
            Suspension(hit);
            
            Steering();
            
            Acceleration();
        }
        else
        {
            _springLength = _maxLength;
        }
    }

    private void WheelMeshVision()
    {
        _wheelCurrentAngle = Mathf.Lerp(_wheelCurrentAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * _wheelCurrentAngle);
        
        wheelMesh.localPosition = new Vector3(0, -_springLength, 0);
        
        wheelMesh.Rotate(Vector3.right,Vector3.Dot(transform.root.transform.forward, _rigidbody.velocity)*50 * Time.fixedDeltaTime);
    }
    

    public void Suspension(RaycastHit hit)
    {
        _lastSpringLength = _springLength;
        _springLength = hit.distance - wheelRadius;
        _springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);

        _springVelocity = (_lastSpringLength - _springLength) / Time.fixedDeltaTime;

        _springForce = springStiffness * (restLength - _springLength);
        _dampingForce = dampingStiffness * _springVelocity;
        _suspensionForce = (_springForce + _dampingForce) * transform.up;

            
        _rigidbody.AddForceAtPosition(_suspensionForce, transform.position);
        
        
        DrawArrow.ForDebug(transform.position, _suspensionForce.normalized, Color.cyan);
    }

    public void Steering()
    {
        Vector3 steeringDir = transform.right;
        Vector3 tireWorldVelocity = _rigidbody.GetPointVelocity(transform.position);
        float steeringVel = Vector3.Dot(steeringDir, tireWorldVelocity);
        // tGripFactor = gripFactor.Evaluate(Mathf.Clamp01(steeringVel / tireWorldVelocity.magnitude));
            
        float desiredVelChange = -steeringVel * wheelGripFactor;
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            
        Vector3 gripForce = steeringDir* desiredAccel*_rigidbody.mass * 0.05f * 0.1f;
            
        _rigidbody.AddForceAtPosition(gripForce, transform.position);
        
        
        DrawArrow.ForDebug(transform.position, gripForce.normalized, Color.yellow);
    }

    public void Acceleration()
    {
        Vector3 accelDir = transform.forward;
            
        accelInput = (accelInput >= 0f) ? 0.8f : -0.6f;
        float carSpeed = Vector3.Dot(transform.root.transform.forward, _rigidbody.velocity);
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);
        float availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelInput;

        var forwardForce = accelDir * availableTorque * _rigidbody.mass*6;
        _rigidbody.AddForceAtPosition(forwardForce, transform.position);
        
        
        DrawArrow.ForDebug(transform.position, forwardForce.normalized, Color.green);
                
       
    }
}