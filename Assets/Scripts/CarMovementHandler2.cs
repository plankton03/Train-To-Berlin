using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CarMovementHandler2 : MonoBehaviour
{
    
    [Header("Input values")]
    public float horizontalInput;
    public float verticalInput;

    [Header("Forward movement variables")]
    public float moveFactor = 30;

    [Header("Turning variables")]
    public float turnFactor;



    [Header("Visual")]
    public float visualFactor;
    public float visualTime;
    
    
    private float _visualAngle;

    
    void Update()
    {
        CheckInput();
        MoveForward();
        Turn();
        Visual();
    }
    
    private void CheckInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = 0.5f;
    }

    private void MoveForward()
    {
        transform.position += (verticalInput * Time.deltaTime * transform.forward) * moveFactor;
    }

    private void Turn()
    {
        float angle = Mathf.Rad2Deg * (turnFactor * horizontalInput * Time.deltaTime);
        transform.Rotate(transform.up, angle);
    }

    private void Visual()
    {
        _visualAngle = Mathf.Lerp(_visualAngle, turnFactor * horizontalInput * visualFactor * Mathf.Rad2Deg,
            visualTime * Time.deltaTime);


        Vector3 transformEulerAngles = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(transformEulerAngles.x, transformEulerAngles.y, _visualAngle);
    }


}