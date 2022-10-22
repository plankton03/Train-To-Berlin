using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CarAssembler : MonoBehaviour
{
    public Equipment equipment;

    private Transform _car;

    public Transform carSpawnPoint;

    public Transform camera;

    private void Start()
    {
        Assemble();
    }

    public void FindSpawnPoint()
    {
        //string based search
        // _carSpawnPoint = GameObject.FindGameObjectWithTag("CarSpawnPoint").transform;
    }

    public void Assemble()
    {
        FindSpawnPoint();
        
        _car = Instantiate(equipment.equipedCar.carPrefab, carSpawnPoint.position, carSpawnPoint.rotation);
        _car.AddComponent<InputManager>().carController = _car.GetComponent<CarController>();

        // GameObject followCamera = GameObject.FindGameObjectWithTag("FollowCamera");
        camera.GetComponent<CinemachineVirtualCamera>().Follow = _car;
        camera.GetComponent<CinemachineVirtualCamera>().LookAt = _car;
    }
}