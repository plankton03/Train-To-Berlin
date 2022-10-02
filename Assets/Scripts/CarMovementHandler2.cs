using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CarMovementHandler2 : MonoBehaviour
{

    public float horizontalInput;

    public float verticalInput;

    public float turnFactor;

    public float steerAngle;

    public float moveFactor = 30;
    

    public float visualFactor;

    public float visualTime;

    public float visualAngle;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        Move();
     Turn();
        Visual();
    }

    void Move()
    {
        transform.position += (verticalInput *Time.deltaTime * transform.forward)* moveFactor;
    }

    void Turn()
    {  
        float angle =Mathf.Rad2Deg * ( turnFactor * horizontalInput * Time.deltaTime);
         transform.Rotate(transform.up , angle);
    }

    void Visual()
    {
        
        visualAngle = Mathf.Lerp(visualAngle, turnFactor * horizontalInput * visualFactor * Mathf.Rad2Deg, visualTime * Time.deltaTime);


        
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,visualAngle);
        // transform.Rotate(transform.forward , visualAngle * 0.2f);
    }
    void CheckInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = 0.5f;
    }
}
