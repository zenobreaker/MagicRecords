using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DangerLine : MonoBehaviour
{
    float currentTime = 0;
    public float lerpTime = 0.5f;
    public Vector3 EndPosition;
    public float distance; //

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    // 일자형 라인을 만든다 
    public void CreateGuideSinlgeLine(WarningSignInfo info)
    {
        if (info == null) return;

        //CreateGuideSinlgeLine(info.startPos, info.quaternion, info.distance);
    }

    public void CreateGuideSinlgeLine(Vector3 startPos, Quaternion rotate, float distance)
    {
        this.distance = distance;
        // 보간 시간이 어느 정도보다 작다면 강제로 최소 1초 정도 보이게 한다. 
        if (distance < 1)
        {
            lerpTime = 1.0f;
        }
  
        var myPosition = gameObject.transform.position;
        Vector3 upPos = myPosition + new Vector3(0, 0.2f, 0); 
        gameObject.transform.position = upPos;

        // 만들면 오브젝트를 켜서 업데이트 함수에서 동작시키게 한다.
        StartCoroutine(CreateLine());
    }



    // 직선으로 나아가는 가이드라인 
    IEnumerator CreateLine()
    {
        gameObject.SetActive(true);
        var lcalZ = 0.0f; //gameObject.transform.localScale.z;
        var startScale = gameObject.transform.localScale;
        var endScale = new Vector3(startScale.x, startScale.y, startScale.z * distance);
        while (lcalZ <= distance)
        {
            currentTime += Time.deltaTime;

            if(currentTime >= lerpTime)
            {
                currentTime = lerpTime;
            }
            // 고정 좌표에서 보간값이 일정하게 하기 때문에 속도가 똑같아지게 하도록 
            gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / lerpTime);
            lcalZ = gameObject.transform.localScale.z;

            yield return null;
        }

        // 바로 꺼지지 않고 어느정도 보이고 꺼지도록 
        yield return new WaitForSeconds(0.5f);
        //범위선이 특정 길이 만큼 길어지면 끄기
        if (distance <= lcalZ)
        {
            gameObject.SetActive(false);
        }
    }

}
