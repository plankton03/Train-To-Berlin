using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpring : MonoBehaviour
{

    public List<GameObject> springs;
    public List<GameObject> wheelMeshes;
    public Rigidbody rigidbody;
    public float maxDistance;
    public float maxForce;
    public float wheelRadius;
    public float dampingFactor;


    public float forwardSpeed;
    private float verticalInput;
    private float horizontalInput;

    public Dictionary<GameObject, GameObject> wheels = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        for (int i = 0; i < springs.Count; i++)
        {
            wheels.Add(springs[i],wheelMeshes[i]);
        }
    }

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        verticalInput = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject wheelMesh;
        
        foreach (GameObject spring in springs)
        {
            bool meshExists = wheels.TryGetValue(spring, out wheelMesh);
            RaycastHit hit;
            if (Physics.Raycast(spring.transform.position,-transform.up,out hit,maxDistance))
            {
                float dampingForce = dampingFactor * Vector3.Dot(rigidbody.GetPointVelocity(spring.transform.position),spring.transform.up);
                float springForce = maxForce * Time.fixedDeltaTime *  Mathf.Max((maxDistance + wheelRadius  - hit.distance) /(maxDistance)-dampingForce,0);
                Debug.DrawRay(spring.transform.position,-transform.up,Color.cyan,hit.distance);
                print(hit.distance);

                float forwardMovementForce = verticalInput * forwardSpeed;
                rigidbody.AddForceAtPosition((springForce )* transform.up + forwardMovementForce * transform.forward,spring.transform.position);
                
                
                if (meshExists)
                {
                    wheelMesh.transform.localPosition = new Vector3(wheelMesh.transform.localPosition.x,
                        (wheelRadius - hit.distance) / wheelMesh.transform.lossyScale.y,
                        wheelMesh.transform.localPosition.z);
                }
            }
            else if (meshExists)
            {
                wheelMesh.transform.localPosition = new Vector3(wheelMesh.transform.localPosition.x,
                    (wheelRadius - maxDistance) / wheelMesh.transform.lossyScale.y,
                    wheelMesh.transform.localPosition.z);
            }
        }
    }
    
        

}
