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
        // �⺻������ �� �Լ��� �˾��� �ݴ´�. 
        cancelButton.onClick.AddListener(() =>
        {
            callback?.Invoke();
            UIPageManager.instance.Cancel(this.gameObject);
        });
    }

}
