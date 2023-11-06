using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

// 이 클래스는  UI의 기본
public class UiBase : MonoBehaviour
{
    [Header("스크롤뷰 부모 오브젝트")]
    public GameObject content;
    [Header("스크롤뷰 자식 오브젝트")]
    public GameObject childObject;  // 스크롤뷰에 넣어질 오브젝트 

    // 상속 받는 클래스는 아래 클래스를 정의하여 쓸 수 있다.
    public virtual void RefreshUI()
    {

    }


    // 스크롤뷰 오브젝트에 원하는 만큼 붙인다. 이미 있다면 붙이지 않음.
    public void InitScrollviewObject(int count = 0 )
    {
        if (content == null || childObject == null) return;

        // 이전에 만든 자식 오브젝트 전부 비활성화
        if(content.transform.childCount > 0)
        {
            for(int i = 0; i < content.transform.childCount; i++)
            {
                if(content.transform.GetChild(i) != null)
                {
                    content.transform.GetChild(i).gameObject.SetActive(false); 
                }
            }
        }

        // count 수 만큼 자식 오브젝트 배치

        // 지정한 수만큼 자식오브젝트가 있다면 생성하지 않는다. 
        // 이미 부모 오브젝트에 지정한 오브젝트 개수 만큼있다면 그것들부터 사용하도록 
        if(content.transform.childCount>= count)
        {
            return; 
        }

        for(int i = 0;i < count; i ++)
        {
            Instantiate(childObject, content.transform);
        }
    }

    // 스크롤뷰 오브젝트에 추가적으로 자식오브젝트 추가하기 
    public void AddScrollviewObject(int count= 0)
    {
        if (content == null || childObject == null) return;

        for(int i = 0; i < count; i ++)
        {
            Instantiate(childObject, content.transform);
        }
    }

    
    // 스크롤뷰 자식 오브젝트 전체에 동일한 콜백을 넣어주는 함수 
    public virtual void SetScrollviewChildObjectsCallack<T>(Action<T> callback)
    {
        if (content == null || childObject == null) return;

        // 부모 오브젝트의 자식들에게 접근
        for (int i = 0; i < content.transform.childCount; i++)
        {
            var childObject = content.transform.GetChild(i);

            if (childObject.TryGetComponent<T>(out T component))
            {
                if(callback != null)
                {
                    callback(component);
                }

            }

        }
    }
}
