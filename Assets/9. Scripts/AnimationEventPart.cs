using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventPart : MonoBehaviour
{
    public Collider eventObject;  // �̺�Ʈ �� ������ ������Ʈ

    // ��� �����̴� ������Ʈ�� �̺�Ʈ
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
