using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : Slot
{
    
    [SerializeField] Text txt_ItemName = null;
    [SerializeField] Text txt_ItemCost = null;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
   
        if (item != null && isClick)
        {
            // 아이템 구매 창
            ShopPage.instance.OpenSaleUI(this);
        }
    }

  

    public override void ClearSlot()
    {
        base.ClearSlot();

        txt_ItemName.text = "";
    }

    public override void AddItem(Item _item, int _count = 1)
    {
        base.AddItem(_item, _count);
        Debug.Log("이게 됨");
        txt_ItemName.text = _item.itemName;
        txt_ItemCost.text = _item.itemValue.ToString();
    }
}
