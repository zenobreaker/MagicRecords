using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float rotateRate;

    void Update()
    {
        transform.Rotate(new Vector3(rotateRate * Time.deltaTime, 0, 0),Space.World);
    }
}
