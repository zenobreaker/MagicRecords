using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

// �� Ŭ������  UI�� �⺻
public class UiBase : MonoBehaviour
{
    [Header("��ũ�Ѻ� �θ� ������Ʈ")]
    public GameObject content;
    [Header("��ũ�Ѻ� �ڽ� ������Ʈ")]
    public GameObject childObject;  // ��ũ�Ѻ信 �־��� ������Ʈ 

    // ��� �޴� Ŭ������ �Ʒ� Ŭ������ �����Ͽ� �� �� �ִ�.
    public virtual void RefreshUI()
    {

    }


    // ��ũ�Ѻ� ������Ʈ�� ���ϴ� ��ŭ ���δ�. �̹� �ִٸ� ������ ����.
    public void InitScrollviewObject(int count = 0 )
    {
        if (content == null || childObject == null) return;

        // ������ ���� �ڽ� ������Ʈ ���� ��Ȱ��ȭ
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

        // count �� ��ŭ �ڽ� ������Ʈ ��ġ

        // ������ ����ŭ �ڽĿ�����Ʈ�� �ִٸ� �������� �ʴ´�. 
        // �̹� �θ� ������Ʈ�� ������ ������Ʈ ���� ��ŭ�ִٸ� �װ͵���� ����ϵ��� 
        if(content.transform.childCount>= count)
        {
            return; 
        }

        for(int i = 0;i < count; i ++)
        {
            Instantiate(childObject, content.transform);
        }
    }

    // ��ũ�Ѻ� ������Ʈ�� �߰������� �ڽĿ�����Ʈ �߰��ϱ� 
    public void AddScrollviewObject(int count= 0)
    {
        if (content == null || childObject == null) return;

        for(int i = 0; i < count; i ++)
        {
            Instantiate(childObject, content.transform);
        }
    }

    
    // ��ũ�Ѻ� �ڽ� ������Ʈ ��ü�� ������ �ݹ��� �־��ִ� �Լ� 
    public virtual void SetScrollviewChildObjectsCallack<T>(Action<T> callback)
    {
        if (content == null || childObject == null) return;

        // �θ� ������Ʈ�� �ڽĵ鿡�� ����
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
