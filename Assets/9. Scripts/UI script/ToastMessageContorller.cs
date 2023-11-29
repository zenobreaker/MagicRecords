using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ToastMessageContorller : MonoBehaviour
{
    // ToastMessage 클래스를 조종하는 컨트롤러 
    // 어느 클래스에서든 이 컨트롤러를 호출해서 게임에서 독립적으로 존재한다.
    [SerializeField]
    static private ToastMessage toastMessage;

    public void Awake()
    {
        // 씬이 로드 될 때 해당 오브젝트도 같이 찾는다. 
        if (toastMessage == null)
        {
            toastMessage  = FindObjectOfType<ToastMessage>();
            toastMessage.gameObject.SetActive(false);
        }
    }

    // 토스트 메세지를 만들어 주고 보여준다.
    static public void CreateToastMessage(string message)
    {
        if (toastMessage == null) return;

        toastMessage.CreateToastMessage(message);
    }

    //public void Update()
    //{
    //    // 테스트용 
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CreateToastMessage("test");
    //    }
    //}
}
