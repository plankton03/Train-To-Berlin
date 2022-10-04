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

    public Dictionary<GameObject, GameObject> wheels = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        for (int i = 0; i < springs.Count; i++)
        {
            wheels.Add(springs[i],wheelMeshes[i]);
        }
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
                float springForce = maxForce * Time.fixedDeltaTime *  Mathf.Max((maxDistance +wheelRadius - hit.distance) /(maxDistance),0);
                Debug.DrawRay(spring.transform.position,-transform.up,Color.cyan,hit.distance);
                rigidbody.AddForceAtPosition((springForce-dampingForce )* transform.up,spring.transform.position);
                if (meshExists)
                {
                    wheelMesh.transform.localPosition = new Vector3(wheelMesh.transform.localPosition.x,
                        (wheelRadius - hit.distance) / wheelMesh.transform.lossyScale.y,
                        wheelMesh.transform.localPosition.z);
                }
            }
        }
    }
}
