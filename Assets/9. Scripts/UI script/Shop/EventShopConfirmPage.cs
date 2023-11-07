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

    // 아이템 선택 시, 구매 확인 UI 
    public void OpenConfirmBuyItemUI(Item itemData)
    {
        if (itemData == null) return;

        // 아이템 정보를 그린다. 
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

    // 위에 함수로 열릴 UI 확인 버튼에 들어갈 이벤트 
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
