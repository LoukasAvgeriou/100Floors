using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    float rotationSpeed = 45;
    Vector3 currentEulerAngles;
    public float z = 0.5f;

    private void Update()
    {
        //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles += new Vector3(0, 0, z) * Time.deltaTime * rotationSpeed;

        //apply the change to the gameObject
        transform.eulerAngles = currentEulerAngles;
    }
}
