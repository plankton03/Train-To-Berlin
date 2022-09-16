using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TestController : MonoBehaviour
{
// car physics calculations/input stuff 
    private Vector3 accel;
    public float throttle;
    private float deadzone = .001f;
    private Vector3 myRight;
    private Vector3 velo;
    public Vector3 flatVelo;
    private Vector3 relativeVelocity;
    private Vector3 dir;
    private Vector3 flatDir;
    private Vector3 carUp;
    private Transform carTransform;
    private Rigidbody carRigidbody;
    public Vector3 engineForce;


    private Vector3 turnvec;
    private Vector3 imp;
    private float rev;
    private float actualTurn;
    private float carMass;
    private Transform[] wheelTransform = new Transform[4]; //these are the transforms for our 4 wheels
    public float actualGrip;
    public float horizontal; //horizontal input control, either mobile control or keyboard
    private float maxSpeedToTurn = .2f; //keeps car from turning until it's reached this value

// the physical transforms for the car's wheels 
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

//these transform parents will allow wheels to turn for steering/separates steering turn from acceleration turning
    public Transform LFWheelTransform;
    public Transform RFWheelTransform;

// car physics adjustments
    public float power = 300;
    public float maxSpeed = 50;
    public float carGrip = 70;
    public float turnSpeed = 3.0f;

//keep this value somewhere between 2.5 and 6.0
    private float slideSpeed;
    public float mySpeed;

    private Vector3 carRight;
    private Vector3 carFwd;
    private Vector3 tempVEC;
    public Transform centerOfMass;


    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Cache a reference to our car's transform
        carTransform = transform;
        // cache the rigidbody for our car
        carRigidbody = GetComponent<Rigidbody>();
        // cache our vector up direction
        carUp = carTransform.up;
        // cache the mass of our vehicle
        carMass = carRigidbody.mass;

        // cache the Forvard World Vector for our car
        carFwd = Vector3.forward;
        // cache the World Right Vector for our car
        carRight = Vector3.right;
        // call to set up our wheels array
        setUpWheels();

        //negative value in Y axis to prevent car from flipping over
        carRigidbody.centerOfMass = new Vector3(0f, -0.7f, .35f);
        //carRigidbody.centerOfMass = centerOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // call the function to start processing all vehicle physics
        carPhysicsUpdate();
        //call the function to see what input we are using and apply it
        checkInput();
    }

    private void LateUpdate()
    {
        // this function makes the visual 3d wheels rotate and turn
        rotateVisualWheels();
        //this is where we send to a function to do engine sounds
        //engineSound();
    }

    public void setUpWheels()
    {
        if (null == frontLeftWheel || null == frontRightWheel || null == rearLeftWheel || null == rearRightWheel)
        {
            Debug.LogError("One or more of the wheel transforms have not been plugged in on the car");
            Debug.Break();
        }
        else
        {
            wheelTransform[0] = frontLeftWheel;
            wheelTransform[1] = rearLeftWheel;
            wheelTransform[2] = frontRightWheel;
            wheelTransform[3] = rearRightWheel;
        }
    }


    private Vector3 rotationAmount;

    public void rotateVisualWheels()
    {
        // front wheels visual rotation while steering the car
        LFWheelTransform.localEulerAngles = new Vector3(LFWheelTransform.localEulerAngles.x, horizontal * 30,
            LFWheelTransform.localEulerAngles.z);
        RFWheelTransform.localEulerAngles = new Vector3(RFWheelTransform.localEulerAngles.x, horizontal * 30,
            RFWheelTransform.localEulerAngles.z);


        rotationAmount = carRight * (relativeVelocity.z * 1.6f * Time.deltaTime * Mathf.Rad2Deg);
        wheelTransform[0].Rotate(rotationAmount);
        wheelTransform[1].Rotate(rotationAmount);
        wheelTransform[2].Rotate(rotationAmount);
        wheelTransform[3].Rotate(rotationAmount);
    }

    private float deviceAccelerometersensitivity = 2; //how sensitive our mobile accelerometer will be

    public void checkInput()
    {
        //Mobile platform turning input... testing to see if we are on a mobile device.
        if (Application.platform == RuntimePlatform.IPhonePlayer || (Application.platform == RuntimePlatform.Android))
        {
            if (accel.x > deadzone || accel.x < -deadzone)
            {
                horizontal = accel.x;
            }
            else
            {
                horizontal = 0;
            }

            throttle = 0;


            foreach (var touch in Input.touches)
            {
                if (touch.position.x > Screen.width - Screen.width / 3 && touch.position.y < Screen.height / 3)
                    throttle = 1;
                else if (touch.position.x < Screen.width / 3 && touch.position.y < Screen.height / 3)
                    throttle = -1;
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor ||
                 Application.platform == RuntimePlatform.WindowsPlayer)
        {
            //Use the Keyboard for all car input
            horizontal = Input.GetAxis("Horizontal");
            throttle = Input.GetAxis("Vertical");
        }
    }

    public void carPhysicsUpdate()
    {
        //grab all the physics info we need to calc everything
        myRight = carTransform.right;

        // find our velocity
        velo = carRigidbody.velocity;
        // velo = new Vector3(0, 0, 1);
        tempVEC = new Vector3(velo.x, 0, velo.z);
        // figure out our velocity without y movement - our flat velocity

        flatVelo = tempVEC;
        // find out which direction we are moving in
        dir = transform.TransformDirection(carFwd);
        tempVEC = new Vector3(dir.x, 0, dir.z);


        // calculate our direction, removing y movement - our flat direction
        flatDir = Vector3.Normalize(tempVEC);
        // calculate relative velocity
        relativeVelocity = carTransform.InverseTransformDirection(flatVelo);
// calculate how much we are sliding (find out movement along our x axis)
        slideSpeed = Vector3.Dot(myRight, flatVelo);
// calculate current speed (the magnitude of the flat velocity)
        mySpeed = flatVelo.magnitude;
// check to see if we are moving in reverse
        rev = Mathf.Sign(Vector3.Dot(flatVelo, flatDir));
// calculate engine force with our flat direction vector and acceleration
        engineForce = (flatDir * (power * throttle) * carMass);
// do turning actualTurn = horizontal;
        actualTurn = horizontal;
        // if we're in reverse, we reverse the turning direction too
        if (rev < 0.15)
            actualTurn = -actualTurn;
// calculate torque for applying to our rigidbody
        turnvec = (((carUp * turnSpeed) * actualTurn) * carMass) * 800;
// calculate impulses to simulate grip by taking our right vector, reversing the slidespeed and
// multiplying that by our mass, to give us a completely 'corrected' force that would completely
// stop sliding. We then multiply that by our grip amount (which is, technically, a slide amount) which
// reduces the corrected force so that it only helps to reduce sliding rather than completely
// stop it
        actualGrip = Mathf.Lerp(1, carGrip, mySpeed * 0.02f);
        imp = myRight * (-slideSpeed * carMass * actualGrip);
    }

    public void slowVelocity()
    {
        carRigidbody.AddForce(-flatVelo * 0.8f);
    }

//this controls the sound of the engine audio by adjusting the pitch of our sound file
    public void engineSound()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.pitch = 0.30f + mySpeed * 0.025f;
            if (mySpeed > 30)
                audio.pitch = 0.25f + mySpeed * 0.015f;
            if (mySpeed > 40)
                audio.pitch = 0.20f + mySpeed * 0.013f;
            if (mySpeed > 49)
                audio.pitch = 0.15f + mySpeed * 0.011f;
//ensures we dont exceed to crazy of a pitch by resetting it back to default 2 if ( audio.pitch > 2.0 ) {
            audio.pitch = 2.0f;
        }
    }

    
    
    void FixedUpdate()
    {
        if (mySpeed < maxSpeed)
           // print(engineForce * Time.deltaTime);
// apply the engine force to the rigidbody
            carRigidbody.AddForce(engineForce * Time.deltaTime);
//if we're going to slow to allow kart to rotate around
        if (mySpeed > maxSpeedToTurn)
// apply torque to our rigidbody
            carRigidbody.AddTorque(turnvec * Time.deltaTime);
        else if (mySpeed < maxSpeedToTurn)
            return;
// apply forces to our rigidbody for grip
        carRigidbody.AddForce(imp * Time.deltaTime);
    }
}