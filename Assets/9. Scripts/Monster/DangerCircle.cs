using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DangerCircle : MonoBehaviour
{
    GameObject waringCircle;

    public class WarningCircleInfo
    {
        public float angle;
        public float radius; 
        public quaternion rotaion;

        
    }


    void CreateWarningcircle(float angle, float radius, quaternion rotation)
    { 
        WarningCircleInfo info = new WarningCircleInfo();
    }

}
