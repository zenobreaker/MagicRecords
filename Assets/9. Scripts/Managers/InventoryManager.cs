using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public struct DataItem
{
    public int idx;
    public EquipItem item;

    public DataItem(int _idx, EquipItem _item)
    {
        idx = _idx;
        item = _item;
    }
}

public class InventoryManager:MonoBehaviour
{
    public static InventoryManager instance;

    public List<DataItem> equipItems = new List<DataItem>(); // 장착한 아이템

    //List<InvenSlot> equipedSlots = new List<InvenSlot>(); // 장착한 아이템이 들어 있는 슬롯
    public EquipItem TestItem;
    public EquipItem testDrone; 

    [SerializeField] Inventory inventory = null;
    [SerializeField] ItemDatabase itemDatabase = null;
   
   // [SerializeField] ItemEffectDatabase theDatabase = null; 

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        if (itemDatabase == null)
        {
            itemDatabase = FindObjectOfType<ItemDatabase>();
        }
    }

    // 테스트용 기능 함수 
    public void TestAddButton()
    {
        var testItem = CreateItem("equipment_weapon_gun_0");
        inventory.AddItem(testItem);

        LobbyManager.MyInstance.IncreseCoin(100000);
    }

    // 테스트용 기능 함수 
    public void TestAddDroneButton()
    {
       var testItem = CreateItem("equipment_drone_1");
        inventory.AddItem(testItem);
    }


    // 아이템 넣기 
    public void AddItemToInven(Item _item, int _count = 1)
    {
        if (inventory == null) return;

        inventory.AddItem(_item);
    }

    // 아이템 생성 (인벤토리에서 생성한다니?!!)
    public Item CreateItem(string _keycode)
    {
        // 키코드를 통해서 아이템데이터베이스에서 아이템을 만들어 반환함
        if (itemDatabase == null) return null;

        var item = itemDatabase.GetItem(_keycode);

        return item;
    }

    // 아이템 정보 갱신 
    public void RefreshItemInfo(ref Item _item)
    {
        if (_item == null) return;

        // 인벤토리에서 해당 아이템을 찾아 정보를 갱신한다. 
        // 연결한 데이터 베이스가 없으므로 해당 아이템 매개변수를 받아서 받아온 정보를 토대로 인벤토리를 수정
        // TODO 데이터베이스가 연결되어 있다면 장착했을 때 통신을 통해 이미 데이터베이스에 올라온 데이터를 
        // 가져와서 받아온 정보를 수정 

        if (inventory == null) return;

        var list = inventory.GetItemByInventory(_item); 
        foreach (var item in list)
        {
            if(item.itemUID == _item.itemUID)
            {
                item.userID = _item.userID;
                var equipItem = item as EquipItem;
                var refEquipItem = _item as EquipItem;
                //  장비면 ? 
                if (equipItem != null && refEquipItem != null)
                {
                    // 장착 정보 갱신 
                    equipItem.isEquip = refEquipItem.isEquip;
                    equipItem.itemEnchantRank = refEquipItem.itemEnchantRank; 
                    equipItem.itemMainAbility = refEquipItem.itemMainAbility;
                    equipItem.SetSubAbility(refEquipItem.itemAbilities);
                }

                // 나머진 생각나면 추가하자
            }
        }
    }


    // 아이템 착용 
    public void EquipItem(Character p_targetPlayer, InvenSlot _invenSlot)
    {
        if(p_targetPlayer == null || _invenSlot == null)
        {
            Debug.Log("대상 캐릭터나 슬롯이 null");
            return; 
        }

        EquipItem t_item = (EquipItem)_invenSlot.GetItem();
        int t_idx = 0; // inventory.GetSlotList().IndexOf(_invenSlot.GetItem() as EquipItem);
        Debug.Log(_invenSlot.GetItem().itemName + " " + t_idx);

        //Player t_Player = InfoManual.MyInstance.GetSelectedPlayer();

        int count = 0; // p_targetPlayer.MyEquipItems.Length;
        Debug.Log("장착루틴");
        for (int i = 0; i < count; i++)
        {
            //if (p_targetPlayer.MyEquipItems[i] != null)    // 해당 부위에 장착된게 있다면
            //{
            //    EquipItem idxEquipItem = p_targetPlayer.MyEquipItems[i];
            //    TakeOffEquipment(p_targetPlayer, idxEquipItem);

            //    p_targetPlayer.SetEmptyEquip(idxEquipItem);
            //   // InfoManual.MyInstance.SetEquipSlot();
            //    break;
            //}
        }

        if (!t_item.isEquip)
        {
            Debug.Log("장착!");
            DataItem t_DataItem = new DataItem(t_idx, t_item);
            equipItems.Add(t_DataItem);

            //p_targetPlayer.EquipItem(t_item);
            _invenSlot.EquipingItemSlot();
            t_item.userID = p_targetPlayer.MyID;
        }

    }

    public void TakeOffEquipment(EquipItem p_item)
    {
        if (p_item == null) return;

        var player = InfoManager.instance.GetMyPlayerInfo((int)p_item.userID);
        if(player == null) return;

        TakeOffEquipment(player, p_item);
    }

    // 장비 아이템 해제 
    public void TakeOffEquipment(Character _target, EquipItem p_item)
    {
        if (p_item == null || _target == null)
            return;

       // var equipItems = _target.MyEquipItems;

        // 장착한 아이템의 인벤토리에 등재된 번호를 가져옴. 
        foreach (var f_item in equipItems)
        {
            //if (f_item.equipTagetUniqueID == p_item.equipTagetUniqueID)
            //{
            //    //t_idx = p_item.idx;
            //    //inventory.TakeOffEquipment(t_idx);
            //    //equipItems.Remove(f_item);
            //    //_targetPlayer.sez
            //    //p_item.equipTarget.SetEmptyEquip(f_item.item);
            //    //InfoManual.MyInstance.GetSelectedPlayer().SetEmptyEquip(p_item);
            //    //InfoManual.MyInstance.SetEquipSlot();

            //    EquipManager.instance.ApplyAbilityEquip(_target, p_item, false);
            //    _target.SetEmptyEquip(p_item);
            //    p_item.equipTagetUniqueID = 0;
            //    break;
            //}
        }
    }

    //public void ChangedEquipItem(InvenSlot p_targetSlot)
    //{
    //    EquipItem[] t_Items = InfoManual.MyInstance.GetSelectedPlayer().MyEquipItems;

    //    for (int i = 0; i < t_Items.Length; i++)
    //    {
    //        if((p_targetSlot.MyItem as EquipItem).equipType == t_Items[i].equipType)
    //        {
    //            //TakeOffEquipment(t_Items[i]);
    //            //EquipItem(p_targetSlot);
    //            break;
    //        }
    //    }
    //}


    public void ApplySaveItemData(Dictionary<int, ItemData> saveDatas)
    {
        if (saveDatas == null || itemDatabase == null) return;

        foreach(var data in saveDatas)
        {
            var item = itemDatabase.GetItemByUID(data.Value.itemID);
            if (item == null) continue;

            item.uniqueID = data.Value.uniqueID;

            item.itemRank = data.Value.itemRank;
            item.itemCount = data.Value.count;

            if(data.Value.itemType == ItemType.Equipment && item is EquipItem)
            {
                var equipItem = item as EquipItem;

                equipItem.equipType = data.Value.equipType;
                equipItem.userID = data.Value.userID;
                if (equipItem.userID != 0)
                {
                    equipItem.isEquip = true; 
                }

                equipItem.itemEnchantRank = data.Value.enhanceCount;
                equipItem.itemMainAbility = data.Value.itemMainAbility;
                equipItem.itemAbilities = data.Value.itemAbilities;
            }


            Inventory.instance.AddItem(item);
        }
    }
}
