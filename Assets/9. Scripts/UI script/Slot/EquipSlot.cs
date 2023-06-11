using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipSlot : Slot
{
    public  EquipType equipType;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (item == null)
        {
          
        }
        else
        {
            ShowChangeMessage();
        }

    }

    // 장비아이템 착용
    public void EquipItem(Item p_item)
    {
        item = p_item;
        AddItem(p_item);
        Debug.Log("장착됨 " + p_item.itemName);
    }
   

    public void ClearEquipSlot()
    {
        item = null;
        ClearSlot();
    }

    
    // 아이템 선택시 보여지는 메시지 
    void ShowChangeMessage()
    {

        // 툴팁 호출
        UIPageManager.instance.OpenToolTip(this);
        //theSlotTooltip.ChangeBtn(2);
        //theSlotTooltip.ShowToolTip(item);
    }
}
