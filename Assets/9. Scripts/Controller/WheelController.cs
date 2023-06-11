using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public GameObject wheels;
    public bool motor;
    public bool steering;
}

public class WheelController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;

    public void RollingWheel(bool p_move, int p_speed = 20)
    {
        float motor;

        if (p_move)
        {
            motor = maxMotorTorque * p_speed;
        }
        else
        {
            motor = 0;
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.wheels.transform.Rotate(motor * Time.deltaTime,0,0);
        }

    }
}
