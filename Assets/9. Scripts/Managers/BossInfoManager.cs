using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BossInfoManager : MonoBehaviour
{
    public static BossInfoManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this; 
    }


}
