using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyer : MonoBehaviour
{
    public static DontDestroyer instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
