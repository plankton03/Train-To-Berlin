using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpring2 : MonoBehaviour
{
    
    
    public List<GameObject> springs;
    public Rigidbody rigidbody;
  
    
    [Header("Suspension")] 
    public float resetLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;

    private Vector3 suspensionForce;

    [Header("Wheel")] 
    //public float wheelRadius;

    private float wheelAngle;
    private Vector3 wheelVelocityLS;
    private float Fx;
    private float Fy;
    void Start()
    {
        minLength = resetLength - springTravel;
        maxLength = resetLength + springTravel;
    }

    
    private void FixedUpdate()
    {
        minLength = resetLength - springTravel;
        maxLength = resetLength + springTravel;
        foreach (GameObject spring in springs)
        {
            if (Physics.Raycast(spring.transform.position,-transform.up , out RaycastHit hit , maxLength))
            {
                Debug.DrawRay(spring.transform.position,-transform.up , Color.cyan , hit.distance);
                lastLength = springLength;
                springLength = hit.distance;
                springLength = Mathf.Clamp(springLength, minLength, maxLength);
                springVelocity = (lastLength - springLength);
                springForce = springStiffness * (resetLength - springLength) ;
                damperForce = damperStiffness * springVelocity;


                suspensionForce = (springForce + damperForce) * transform.up;
                print((suspensionForce * Time.fixedDeltaTime).magnitude);
                rigidbody.AddForceAtPosition(suspensionForce  * Time.fixedDeltaTime ,spring.transform.position);
            }
        }
      
    }
    
}
