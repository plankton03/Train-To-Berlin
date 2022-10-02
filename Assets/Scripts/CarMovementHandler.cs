using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CarMovementHandler : MonoBehaviour
{
    private Rigidbody _rigidBody;

    public float power;
    public float throttle;
    public float turnSpeed;
    public float carGrip;

    private Vector3 imp;
    public float maxSpeed;
    public float mySpeed;
    public float maxSpeedToTurn;
    private Vector3 turnVector;

    private Vector3 engineForce;

    public float horizontalInput;





    public float reverse;
    
    
     
    void Start()
    {
        initialize();
    }
    

    void Update()
    {
        CarPhysicsUpdate();
        
        CheckInput();
        
    }

    private void CheckInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        throttle = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        
        
        if (mySpeed < maxSpeed)
            _rigidBody.AddForce(engineForce * Time.deltaTime);
        
        if (mySpeed > maxSpeedToTurn)
            _rigidBody.AddTorque(turnVector * Time.deltaTime);
        else if (mySpeed < maxSpeedToTurn)
            return;

        _rigidBody.AddForce(imp * Time.deltaTime);
    }

    private void initialize()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.centerOfMass = new Vector3(0f, -0.3f, 0f);
    }

    private void CarPhysicsUpdate()
    {
            Vector3 velocity = _rigidBody.velocity;
            velocity = new Vector3(velocity.x, 0, velocity.z);

            Vector3 direction = transform.TransformDirection(Vector3.forward);
            direction = Vector3.Normalize(new Vector3(direction.x,0,direction.z));

            float slideSpeed = Vector3.Dot(transform.right, velocity);
             mySpeed = velocity.magnitude;



             reverse = Mathf.Sign(Vector3.Dot(velocity, direction));

             engineForce =  (power * throttle) * _rigidBody.mass * direction;

            float actualTurn = horizontalInput;

            if (reverse < 0.15)
            {
                actualTurn = -actualTurn;
            }

            turnVector = (((transform.up * turnSpeed) * actualTurn )* _rigidBody.mass) * 800;

            float actualGrip = Mathf.Lerp(1, carGrip, mySpeed* 0.02f);

            imp = transform.right * (-slideSpeed * _rigidBody.mass * actualGrip);

    }
}
