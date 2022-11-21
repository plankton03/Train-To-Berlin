using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class CarDefinition : ScriptableObject
{
    public string carName; 
    public string description;

    public Transform carPrefab;

    public CarFeatures baseFeatures;
    
}

[Serializable]
public class CarFeatures
{
    public float handling;
    public float maxSpeed;
    public float rawPower;
    public float health;
    public float physicalArmor;
    public float bioArmor;
    public float price;
}