using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRolling : MonoBehaviour
{
    public GameObject wheel;
    public float maxMotorTorque;
    
    // Update is called once per frame
   public void RollingWheel(float p_Speed)
    {
        wheel.transform.Rotate(maxMotorTorque * p_Speed * Time.deltaTime, 0, 0);
    }
}
