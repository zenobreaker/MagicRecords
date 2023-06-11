using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotX;
    public float rotY;
    public float rotZ;

    void Update()
    {
        transform.Rotate(new Vector3(rotX, rotY, rotZ) * Time.deltaTime);
    }
}
