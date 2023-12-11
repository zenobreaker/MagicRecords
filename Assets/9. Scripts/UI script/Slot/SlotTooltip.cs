using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 장착 관련 기능이나 아이템 툴팁 
public class SlotTooltip : UiBase
{
    [SerializeField] protected GameObject go_Base = null;

    [SerializeField] protected Image img_item = null;
    [SerializeField] protected Text txt_ItemName = null;
    [SerializeField] protected Text txt_ItemAbility = null;
    [SerializeField] protected Text txt_ItemSubAb = null;
    [SerializeField] protected Text txt_ItemDesc = null;

    [SerializeField] protected Text txt_ItemHowtoUsed = null;

    [SerializeField] Button btn_Equip = null;
    [SerializeField] Button btn_Change = null;
    [SerializeField] Button btn_TakeOff = null;


    //[SerializeField] ItemEffectDatabase theDatabase = null;
    [SerializeField] Inventory inventory = null;
    [SerializeField] EnchantManual theEnchant = null;
    [SerializeField] ChoiceAlert choiceAlert = null;

    public Item selectedItem;
    public Slot selectedSlot;
    protected int itemCount;

    public Character selectedCharacter; 

    public delegate void EndDelegate();
    EndDelegate endDelegate;    // 팝업이 종료 시에 실행되는 델리게이트 


    private void OnEnable()
    {
        endDelegate = null;
    }

    private void OnDisable()
    {
        selectedItem = null;
        itemCount = 0; 

        if(endDelegate != null)
        {
            endDelegate.Invoke();
        }
    }

    public override void RefreshUI()
    {
        base.RefreshUI();

        // 현재 보여준 아이템 갱신해서 다시 그리기 위한 소스
        ShowToolTip(selectedItem);
    }

    public void SetDelegate(EndDelegate _callback)
    {
        if(_callback != null)
        {
            endDelegate += _callback;
        }
    }

    // 아이템의 이미지를 보여주는 메소드
    public void ShowToolTip(Item p_item)
    {
        if (p_item == null) return; 

        selectedItem = p_item;

        if (!go_Base.activeSelf)
            UIPageManager.instance.OpenClose(go_Base);

        img_item.sprite = p_item.itemImage;
        txt_ItemDesc.text = p_item.itemDesc;


        if(p_item.itemType == ItemType.Equipment)
        {
            EquipItem t_SelectedEquipItem = p_item as EquipItem;
            if (t_SelectedEquipItem != null)
            {
                img_item.sprite = t_SelectedEquipItem.itemImage;
                txt_ItemName.text = "+" + t_SelectedEquipItem.itemEnchantRank + " " + p_item.itemName;
                txt_ItemDesc.text = t_SelectedEquipItem.itemDesc;
                WritingItemAbility(t_SelectedEquipItem);
                WritingItemSubAbility(t_SelectedEquipItem);

                if (!t_SelectedEquipItem.isEquip)
                {
                    Debug.Log("장착 안된 아이템 ");
                    ChangeBtn(1);
                }
                else
                {
                     Debug.Log("장착된 아이템 ");
                   ChangeBtn(3);
                }
            }
        }

    }


    public void ShowToolTip(Item p_item, int _count = 1)
    { 

        selectedItem = p_item;
        itemCount = _count;

        ShowToolTip(p_item);
    }

    public void ShowToolTip(Slot _invenSlot)
    {

        selectedItem = _invenSlot.GetItem();
        selectedSlot = _invenSlot;

        ShowToolTip(selectedItem, selectedItem.itemValue);
    }

    public void WritingItemAbility(EquipItem p_EquipItem)
    {
        if (p_EquipItem.equipType == EquipType.WEAPON)
            txt_ItemAbility.text = "공격력 + " + p_EquipItem.itemMainAbility.power.ToString();
        if (p_EquipItem.equipType == EquipType.ARMOR)
            txt_ItemAbility.text = "방어력 + " + p_EquipItem.itemMainAbility.power.ToString();
        if (p_EquipItem.equipType == EquipType.ACCSESORRY_1)
            txt_ItemAbility.text = "특정 옵션 + " + p_EquipItem.itemMainAbility.power.ToString();
    }

    public void WritingItemAbility(InvenSlot _invenSlot)
    {
        //EquipItem _item = _invenSlot.GetItem() as EquipItem;

        //if (_item.equipType == EquipType.Weapon)
        //    txt_ItemAbility.text = "공격력 + " + _item.itemMainAbility.ToString();
        //if (_item.equipType == EquipType.Armor)
        //    txt_ItemAbility.text = "방어력 + " + _item.itemMainAbility.ToString();
        //if (_item.equipType == EquipType.Accessory)
        //    txt_ItemAbility.text = "특정 옵션 + " + _item.itemMainAbility.ToString();
    }

    public void WritingItemSubAbility(EquipItem p_EquipItem)
    {
        txt_ItemSubAb.text = "";
        for (int i = 0; i < p_EquipItem.itemAbilities.Length; i++)
        {
            txt_ItemSubAb.text += GetSubAbility(p_EquipItem.itemAbilities[i].abilityType)
                + " +" + p_EquipItem.itemAbilities[i].power + "% \n";
        }
    }

    string GetSubAbility(AbilityType p_AbType)
    {
        switch (p_AbType)
        {
            case AbilityType.ATK:
                return "공격력";
            case AbilityType.ASPD:
                return "공격속도";
            case AbilityType.DEF:
                return "방어력";
            case AbilityType.SPD:
                return "이동속도";
            case AbilityType.HP:
                return "체력";
            case AbilityType.HPR:
                return "체력 재생";
            case AbilityType.MP:
                return "마나";
            case AbilityType.MPR:
                return "마나 재생";
        }

        return "";
    }

    public void HideToolTip()
    {
        if (go_Base.activeSelf)
        {
            UIPageManager.instance.OpenClose(go_Base);
            this.selectedItem = null;
        }
    }

    // 아이템 장착
    public void EquipItem()
    {
        if (choiceAlert == null) return; 

        //choiceAlert.ActiveAlert(true);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.INVENTORY;
        choiceAlert.SetSlot(selectedSlot);
        //InventoryManager.instance.EquipItem(selectedSlot);
        //HideToolTip();

        // 캐릭터 선택 UI 호출
        if (selectedCharacter == null || selectedCharacter.MyID == 0)
        {
            UIPageManager.instance.OpenSelectCharacter((selectPlayers) =>
            {
                var selectPlayer = selectPlayers.First();
                if (selectPlayer == null) 
                    return; 
                if (selectPlayer != null && selectedItem != null)
                {
                    if (selectedItem is EquipItem == true)
                    {
                        (selectedItem as EquipItem).isEquip = true;
                        // 장비 장착시키기 
                        EquipManager.instance.RunEquipItem(selectPlayer,
                            (selectedItem as EquipItem).equipType,
                            selectedItem);

                        // 툴팁창을 다시 그린다. 
                        ShowToolTip(selectedSlot);
                    }
                }
            });
        }
        else
        {
            // 선택된 캐릭터가 있으면 바로 장착시킨다. 
            (selectedItem as EquipItem).isEquip = true;
            // 장비 장착시키기 
            EquipManager.instance.RunEquipItem(selectedCharacter,
                (selectedItem as EquipItem).equipType,
                selectedItem);

            // 툴팁창을 다시 그린다. 
            ShowToolTip(selectedSlot);
        }
    }

    // 아이템 변경 
    public void ChangeItem()
    {
        // 아이템의 장착자 id 가져오기
        var id = selectedItem.userID;

        var player = InfoManager.instance.GetPlayerInfo(id);
        if (player == null) return; 


    }

    // 아이템 해제
    public void TakeOffItem()
    {
        // 아이템의 장착자 id 가져오기
        var id = selectedItem.userID;
        var slotType = EquipType.NONE;
        if (selectedItem != null && selectedItem is EquipItem)
        {
            slotType = (selectedItem as EquipItem).equipType;
            // 아이템 해제니 장비면 변경
            (selectedItem as EquipItem).isEquip = false; 
            selectedItem.userID = 0;
        }

        var player = InfoManager.instance.GetPlayerInfo(id);
        if (player == null) return;

        // 아이템 정보 갱신 
        InventoryManager.instance.RefreshItemInfo(ref selectedItem);

        // 장착 해제로 값 전달
        EquipManager.instance.RunEquipItem(player, slotType, null);

        ShowToolTip(selectedSlot);
    }

    // 호출형태에 따른 버튼 변경
    public void ChangeBtn(int p_num)
    {
        switch (p_num)
        {
            case 1: // 장착 안된  장비아이템
                btn_Equip.gameObject.SetActive(true);
                btn_Change.gameObject.SetActive(false);
                btn_TakeOff.gameObject.SetActive(false);
                break;
            case 2: // 장착된 상태에서 다른 장비 아이템 
                btn_Equip.gameObject.SetActive(false);
                btn_Change.gameObject.SetActive(true);
                btn_TakeOff.gameObject.SetActive(false);
                break;
            case 3: // 장착한 아이템 
                btn_Equip.gameObject.SetActive(false);
                btn_Change.gameObject.SetActive(false);
                btn_TakeOff.gameObject.SetActive(true);
                break;
        }

    }

    public void EnchantBtn()
    {
        if (theEnchant == null) return; 

        UIPageManager.instance.OpenClose(theEnchant.go_BaseUI);
        theEnchant.SetEquipItemInfo(selectedItem as EquipItem);
        //HideToolTip();
    }


    public void CancelUI()
    {
        selectedItem = null;
        selectedSlot = null;
        HideToolTip();
    }
}
