using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticePopup : UiBase
{
    public Button confirmButton;
    public Button cancelButton;

    public delegate void Callback();

    public void Confirm(Callback callback)
    {
        confirmButton.onClick.AddListener(() =>
        {
            callback?.Invoke();
        });
    }

    public void Cancel(Callback callback)
    {
        // 기본적으로 이 함수는 팝업을 닫는다. 
        cancelButton.onClick.AddListener(() =>
        {
            callback?.Invoke();
            UIPageManager.instance.Cancel(this.gameObject);
        });
    }

}
