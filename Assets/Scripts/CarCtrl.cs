using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCtrl : MonoBehaviour
{

    public ScrWheel[] wheels;

    [Header("car specs")] 
    public float wheelBase;
    public float rearTrack;
    public float turnRadius;

    [Header("Inputs")]
    public float steerInput;


    private float ackermannAngleLeft;
    private float ackermannAngleRight;
    
  
    
    void Update()
    {
        steerInput = Input.GetAxis("Horizontal");
        if (steerInput > 0)
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
        }else if (steerInput < 0)
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
        }
        else
        {
            ackermannAngleLeft = 0;
            ackermannAngleRight = 0;
        }

        foreach (ScrWheel wheel in wheels)
        {
            if (wheel.wFL)
            {
                wheel.steerAngle = ackermannAngleLeft;
            }else if (wheel.wFR)
            {
                wheel.steerAngle = ackermannAngleRight;
            }
        }
    }
}

/*
 * int it n n n  n n n 
 *
 * 
*/