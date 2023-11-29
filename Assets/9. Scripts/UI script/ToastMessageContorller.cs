using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ToastMessageContorller : MonoBehaviour
{
    // ToastMessage Ŭ������ �����ϴ� ��Ʈ�ѷ� 
    // ��� Ŭ���������� �� ��Ʈ�ѷ��� ȣ���ؼ� ���ӿ��� ���������� �����Ѵ�.
    [SerializeField]
    static private ToastMessage toastMessage;

    public void Awake()
    {
        // ���� �ε� �� �� �ش� ������Ʈ�� ���� ã�´�. 
        if (toastMessage == null)
        {
            toastMessage  = FindObjectOfType<ToastMessage>();
            toastMessage.gameObject.SetActive(false);
        }
    }

    // �佺Ʈ �޼����� ����� �ְ� �����ش�.
    static public void CreateToastMessage(string message)
    {
        if (toastMessage == null) return;

        toastMessage.CreateToastMessage(message);
    }

    //public void Update()
    //{
    //    // �׽�Ʈ�� 
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CreateToastMessage("test");
    //    }
    //}
}
