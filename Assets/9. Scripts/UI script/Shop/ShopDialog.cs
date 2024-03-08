using UnityEngine;
using UnityEngine.UI;

public class ShopDialog : MonoBehaviour
{
    [SerializeField] protected GameObject go_Base = null;

    [SerializeField] protected Image img_item = null;
    [SerializeField] protected Text txt_ItemName = null;
    [SerializeField] protected Text txt_ItemAbility = null;
    [SerializeField] protected Text txt_ItemDesc = null;

    public Item selectedItem;
    private ShopSlot selectedSlot;
    protected int itemCount;

    public void ShowToolTip(ShopSlot _shopSlot, int _count = 1)
    {
        selectedSlot = _shopSlot;
        selectedItem = _shopSlot.MyItem;
        itemCount = _count;

        if (!go_Base.activeSelf)
            UIPageManager.instance.OpenClose(go_Base);

        img_item.sprite = _shopSlot.MyItem.itemImage;
        //txt_ItemName.text = "+" + _shopSlot.MyItem.itemEnchantRank + " " + _shopSlot.MyItem.itemName;
        //txt_ItemAbility.text = p_item.itemAbility[0].ToString();
        WritingItemAbility();
        txt_ItemDesc.text = _shopSlot.MyItem.itemDesc;
        
    }

    public void WritingItemAbility()
    {
        if (selectedItem.itemType == ItemType.Equipment)
        {
            EquipItem equipItem = selectedItem as EquipItem;
            if(equipItem != null)
            {
                string keycode = "";
                switch(equipItem.itemMainAbility.abilityType)
                {
                    case AbilityType.ATK:
                        keycode = "ATK";
                        break;
                    case AbilityType.DEF:
                        keycode = "DEF";
                        break;
                    case AbilityType.SPD:
                        keycode = "SPD";
                        break;
                    case AbilityType.ASPD:
                        keycode = "ASPD";
                        break;
                    case AbilityType.HPR:
                        keycode = "HPR";
                        break;
                    case AbilityType.MPR:
                        keycode = "MPR";
                        break;
                    case AbilityType.HP:
                        keycode = "HP";
                        break;
                    case AbilityType.MP:
                        keycode = "HP";
                        break;
                }


                txt_ItemAbility.text = keycode + "+ " + equipItem.itemMainAbility.power;
            }

        //    if (selectedItem.equipType == Item.EquipType.Weapon)
        //        txt_ItemAbility.text = "공격력 + " + selectedItem.itemMainAbility.ToString();
        //    if (selectedItem.equipType == Item.EquipType.Armor)
        //        txt_ItemAbility.text = "방어력 + " + selectedItem.itemMainAbility.ToString();
        //    if (selectedItem.equipType == Item.EquipType.Accessory)
        //        txt_ItemAbility.text = ItemEffectDatabase.instance.GetAccesoryItemEffect(selectedItem);
        }
    }

    public void CancelUI()
    {
        selectedItem = null;
        UIPageManager.instance.Cancel(go_Base);
    }

    public void BuyItem()
    {
        if (RLModeController.isRLMode)
        {
            RLModeController.instance.DownBHPoint();
            
            if (RLModeController.instance.behaviourPoint > 0)
            {
                selectedItem.isSale = true;
                selectedSlot.SetItemSale();
                selectedSlot.GetComponent<Image>().enabled = false;
            }
            else
                return;
        }

        if (InventoryManager.coin >= selectedItem.itemValue)
        {
            Item boughtItem = null;
            // todo 나중에 아이템의 타입별로 아이템을 만드는 팩토리같은거 만들자
            boughtItem = ItemDatabase.instance.GetItemByUID(selectedItem.itemUID);

            InventoryManager.instance.AddItemToInven(boughtItem, itemCount);
            LobbyManager.MyInstance.IncreseCoin(-selectedItem.itemValue);

            UIPageManager.instance.Cancel(go_Base);
            Debug.Log("구입 완료" + boughtItem.itemName);
        }
        else
        {
            Debug.Log("구입 실패 " + "잔액이 부족합니다.");
        }
    }
}
