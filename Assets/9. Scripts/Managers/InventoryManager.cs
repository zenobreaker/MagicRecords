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

public class InventoryManager:MonoBehaviour, IRewardObserver
{
    public static InventoryManager instance;

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
       //var testItem = CreateItem("equipment_drone_1");
       // inventory.AddItem(testItem);
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
    public void RefreshItemInfo(ref Item targetItem)
    {
        if (targetItem == null) return;

        // 인벤토리에서 해당 아이템을 찾아 정보를 갱신한다. 
        // 연결한 데이터 베이스가 없으므로 해당 아이템 매개변수를 받아서 받아온 정보를 토대로 인벤토리를 수정
        // TODO 데이터베이스가 연결되어 있다면 장착했을 때 통신을 통해 이미 데이터베이스에 올라온 데이터를 
        // 가져와서 받아온 정보를 수정 

        if (inventory == null) return;

        var list = inventory.GetItemByInventory(targetItem); 
        foreach (var item in list)
        {
            if(item.uniqueID == targetItem.uniqueID)
            {
                item.userID = targetItem.userID;
                var equipItem = item as EquipItem;
                var refEquipItem = targetItem as EquipItem;
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

    public void NotifyReward(List<Item> rewardList)
    {
        foreach(var reward in rewardList)
        {
            AddItemToInven(reward);
        }
    }
}
