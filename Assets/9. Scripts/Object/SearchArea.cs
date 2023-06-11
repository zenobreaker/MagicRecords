using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchArea : MonoBehaviour
{
    ObjectController objCtrl;

    private void Start()
    {
        objCtrl = transform.root.GetComponent<ObjectController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            objCtrl.SetAttackTarget(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        objCtrl.SetAttackTarget(null);
    }
}
