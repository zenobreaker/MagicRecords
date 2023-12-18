using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectEvent : MonoBehaviour
{
    protected void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }

    public void SetDisableTimer(float delay)
    {
        StartCoroutine(DisableObjectAfterDelay(delay));
    }

    IEnumerator DisableObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
