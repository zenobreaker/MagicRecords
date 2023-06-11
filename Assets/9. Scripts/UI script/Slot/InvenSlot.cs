using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvenSlot : Slot
{

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        //Inventory.instance.SetApplyList(EquipMenu.instance.tabNumber);
       // Debug.Log(Inventory.instance.GetSlotList().IndexOf(this));
       

        if (item != null)
        {
            // 툴팁 호출
            UIPageManager.instance.OpenToolTip(this);   
        }
    }


    public bool isEquiped()
    {
        if (item as EquipItem == null) return false; 

        if ((item as EquipItem).isEquip)
            return true;
        else
            return false;
    }

    public void EquipingItemSlot()
    {
        if ((item is EquipItem) == true)
        {
            (item as EquipItem).isEquip = true;
        }
       SetColor(0.5f);
    }

    public void TakeOffItemSlot()
    {
        if ((item is EquipItem) == true)
        {
            Debug.Log("장착 해제하라 " + (item as EquipItem).isEquip);
            (item as EquipItem).isEquip = false;
        }
        SetColor(1);
    }

}
