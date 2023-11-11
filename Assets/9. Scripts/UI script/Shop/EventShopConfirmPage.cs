using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventShopConfirmPage : UiBase
{
    public Image itemImage;

    public TextMeshProUGUI itemNameText;

    public TextMeshProUGUI itemDescriptionText;

    public TextMeshProUGUI itemValueText;

    public Action callback;

    // ������ ���� ��, ���� Ȯ�� UI 
    public void OpenConfirmBuyItemUI(Item itemData)
    {
        if (itemData == null) return;

        // ������ ������ �׸���. 
        if (itemImage != null)
        {
            itemImage.sprite = itemData.itemImage;
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemData.itemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = itemData.itemDesc;
        }

        if (itemValueText != null)
        {
            itemValueText.text = itemData.itemValue.ToString();
        }
    }

    public void SetCallback(Action callback)
    {
        this.callback = callback;
    }

    // ���� �Լ��� ���� UI Ȯ�� ��ư�� �� �̺�Ʈ 
    public void ConfirmBuyItemCallback()
    {
        CloseUi();
        callback?.Invoke();
    }

    public void CloseUi()
    {
        UIPageManager.instance.OpenClose(gameObject);
    }


}

