using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoild : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log(other.transform.name);
            Debug.Log("엔터");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("나감");
            Debug.Break();
        }
    }

    
}
