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

    public bool isOnGround;

    public float tGripFactor;


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

    private Vector3 _forwardForce;
    private Vector3 _rightForce;


    private float wheelPos;


    [Header("Wheel")] public float wheelRadius;
    public float steerAngle;
    public float steerTime;
    public float wheelSpeed;
    public Transform wheelMesh;
    // [Range(0, 1)] public float tireGripFactor;
    public float tireMass;

    public float accelInput;
    public float carTopSpeed;
    public AnimationCurve powerCurve;
    public AnimationCurve gripFactor;

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

        Debug.DrawRay(transform.position, -transform.up * (_springLength + wheelRadius), Color.magenta);
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, _maxLength + wheelRadius))
        {
            _lastSpringLength = _springLength;
            _springLength = hit.distance - wheelRadius;
            _springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);

            _springVelocity = (_lastSpringLength - _springLength) / Time.fixedDeltaTime;

            _springForce = springStiffness * (restLength - _springLength);
            _dampingForce = dampingStiffness * _springVelocity;
            _suspensionForce = (_springForce + _dampingForce) * transform.up;

            _wheelVelocityLocalSpace = transform.InverseTransformDirection(_rigidbody.GetPointVelocity(hit.point));
            _forwardForce = (Input.GetAxis("Vertical") * wheelSpeed) * transform.forward;
            _rightForce = (_wheelVelocityLocalSpace.x * wheelSpeed) * -transform.right;


            // _rigidbody.AddForceAtPosition(_suspensionForce +_forwardForce + _rightForce ,hit.point);

            // _rigidbody.AddForceAtPosition(_suspensionForce ,hit.point);
            _rigidbody.AddForceAtPosition(_suspensionForce, transform.position);


            Vector3 steeringDir = transform.right;
            Vector3 tireWorldVelocity = _rigidbody.GetPointVelocity(transform.position);
            float steeringVel = Vector3.Dot(steeringDir, tireWorldVelocity);
            print(Mathf.Clamp01(steeringVel / tireWorldVelocity.magnitude));
             // tGripFactor = gripFactor.Evaluate(Mathf.Clamp01(steeringVel / tireWorldVelocity.magnitude));
            
            float desiredVelChange = -steeringVel * tGripFactor;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            _rigidbody.AddForceAtPosition(steeringDir * 0.05f * desiredAccel, transform.position);


            Vector3 accelDir = transform.forward;
            
                accelInput = (accelInput >= 0f) ? 0.8f : -0.6f;
                float carSpeed = Vector3.Dot(transform.root.transform.forward, _rigidbody.velocity);
                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);
                float availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelInput;
                // print(availableTorque);
                _rigidbody.AddForceAtPosition(accelDir * availableTorque * _rigidbody.mass*3, transform.position);
            


            SetWheelMeshPos();
            isOnGround = true;
        }
        else
        {
            _springLength = _maxLength;
            isOnGround = false;
            print("Fuck");
        }
    }

    private void SetWheelMeshPos()
    {
        wheelMesh.localPosition = new Vector3(0, -_springLength, 0);
    }

    private void RotateYAxis()
    {
        _wheelCurrentAngle = Mathf.Lerp(_wheelCurrentAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * _wheelCurrentAngle);
    }
}