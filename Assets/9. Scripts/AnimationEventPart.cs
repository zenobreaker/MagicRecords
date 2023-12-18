using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventPart : MonoBehaviour
{
    public Collider eventObject;  // 이벤트 때 동작할 오브젝트

    // 잠깐 깜빡이는 오브젝트용 이벤트
    void FlashAnimationObjectEvent(float delay)
    {
        if (eventObject == null)
            return;
        eventObject.enabled = true;
        SetDisableTimer(delay);
    }



    public void SetDisableTimer(float delay)
    {
        StartCoroutine(DisableObjectAfterDelay(delay));
    }

    IEnumerator DisableObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        eventObject.enabled = false;
    }


}
