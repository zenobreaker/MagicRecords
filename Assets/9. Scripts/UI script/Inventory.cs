using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;


[Serializable]
public enum InventoryCategory
{
    NONE = 0, 
    WEAPON = 1, 
    ARMOR = 2,
    WHEEL = 3,
    ACCSESORRY = 4, 
    DRONE = 5,
    RUNE = 6,
    USED = 7,
    ETC = 8,
    MAX = 8,
}

// 유저의 아이템을 담아놓는 인벤토리 클래스
public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int defaultSlotCount = 100; 
    public int maxSlotCount;        // 최대 인벤 슬롯 개수 

    public Dictionary<InventoryCategory, List<Item>> itemList = new Dictionary<InventoryCategory, List<Item>>();

    //int selectedItem;
    public Item testItem;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        itemList.Add(InventoryCategory.NONE, new List<Item>());
        itemList.Add(InventoryCategory.WEAPON, new List<Item>());
        itemList.Add(InventoryCategory.ARMOR, new List<Item>());
        itemList.Add(InventoryCategory.WHEEL, new List<Item>());
        itemList.Add(InventoryCategory.ACCSESORRY, new List<Item>());
        itemList.Add(InventoryCategory.DRONE, new List<Item>());
        itemList.Add(InventoryCategory.RUNE, new List<Item>());
        itemList.Add(InventoryCategory.USED, new List<Item>());
        itemList.Add(InventoryCategory.ETC, new List<Item>());
    }

    void Start()
    {
        maxSlotCount = defaultSlotCount;
    }


    public List<Item> GetItemsByInventoryType(InventoryCategory _type)
    {
        return itemList[_type];
    }
    
    // 아이템이 담긴 리스트 반환
    public List<Item> GetItems()
    {
        return itemList[InventoryCategory.WEAPON]; 
    }

    public List<Item> GetItemByInventory(Item _item)
    {
        if (_item  == null)
        {
            return null; 
        }

        InventoryCategory inventoryCategory = InventoryCategory.NONE;

        if (_item.itemType == ItemType.Equipment)
        {
            var equipItem = _item as EquipItem;
            if (equipItem != null)
            {
                if (equipItem.equipType == EquipType.WEAPON)
                {
                    inventoryCategory = InventoryCategory.WEAPON;
                }
                else if (equipItem.equipType == EquipType.ARMOR)
                {
                    inventoryCategory = InventoryCategory.ARMOR;
                }
                else if (equipItem.equipType == EquipType.ACCSESORRY_1 ||
                    equipItem.equipType == EquipType.ACCSESORRY_2 ||
                    equipItem.equipType == EquipType.ACCSESORRY_3)
                {
                    inventoryCategory = InventoryCategory.ACCSESORRY;
                }
                else if (equipItem.equipType == EquipType.WHEEL)
                {
                    inventoryCategory = InventoryCategory.WHEEL;
                }
                else if (equipItem.equipType == EquipType.DRONE)
                {
                    inventoryCategory = InventoryCategory.DRONE;
                }
                else if (equipItem.equipType == EquipType.RUNE)
                {
                    inventoryCategory = InventoryCategory.RUNE;
                }

            }
        }
        // 소모품일 경우
        else if (_item.itemType == ItemType.Used)
        {
            inventoryCategory = InventoryCategory.USED;
        }
        // 기타
        else if (_item.itemType == ItemType.ETC)
        {
            inventoryCategory = InventoryCategory.RUNE;
        }

        return GetItemsByInventoryType(inventoryCategory);
    }


    // PRIVATE METHOD : 장비류 아이템을 인벤토리에 넣는다
    private void AddInventoryWithEquipment(Item item)
    {
        if (item == null && item is EquipItem) return;

        EquipItem equipItem = (EquipItem)item;
        InventoryCategory inventoryCategory = InventoryCategory.NONE;

        if (equipItem != null)
        {
            if (equipItem.equipType == EquipType.WEAPON)
            {
                inventoryCategory = InventoryCategory.WEAPON;
            }
            else if (equipItem.equipType == EquipType.ARMOR)
            {
                inventoryCategory = InventoryCategory.ARMOR;
            }
            else if (equipItem.equipType == EquipType.ACCSESORRY_1 ||
                equipItem.equipType == EquipType.ACCSESORRY_2 ||
                equipItem.equipType == EquipType.ACCSESORRY_3)
            {
                inventoryCategory = InventoryCategory.ACCSESORRY;
            }
            else if (equipItem.equipType == EquipType.WHEEL)
            {
                inventoryCategory = InventoryCategory.WHEEL;
            }
            else if (equipItem.equipType == EquipType.DRONE)
            {
                inventoryCategory = InventoryCategory.DRONE;
            }
            else if (equipItem.equipType == EquipType.RUNE)
            {
                inventoryCategory = InventoryCategory.RUNE;
            }

            if(itemList[inventoryCategory].Count < maxSlotCount)
                itemList[inventoryCategory].Add(item);
            else
            {
                // todo 해당 아이템을 넣을 공간이 없다고 메세지를 전달한다.
                Debug.Log(inventoryCategory + "가 꽉차서 넣을 수 없습니다.");
            }
        }
    }

    // PRIVATE METHOD : 장비 외에 아이템을 인벤토리에 넣는다. 
    private void AddInventoryWithETC(Item item)
    {
        if (item == null) return;
        InventoryCategory inventoryCategory = InventoryCategory.NONE;

        if (item.itemType == ItemType.Used)
        {
            inventoryCategory = InventoryCategory.USED;
        }
        else if( item.itemType == ItemType.MATERIAL)
        {
            inventoryCategory = InventoryCategory.ETC;
        }
        // 기타
        else if (item.itemType == ItemType.ETC)
        {
            inventoryCategory = InventoryCategory.ETC;
        }


        // 값 넣어주기 
        if (itemList[inventoryCategory].Count < maxSlotCount)
        {
            // 이미 소지했다면 값을 추가
            if (itemList[inventoryCategory].Find(x => x.itemKeycode == item.itemKeycode) != null)
            {
                itemList[inventoryCategory].Find(x => x.itemKeycode == item.itemKeycode).itemCount += item.itemCount;
            }
            else
            {
                itemList[inventoryCategory].Add(item);
            }
        }
        else
        {
            // todo 해당 아이템을 넣을 공간이 없다고 메세지를 전달한다.
            Debug.Log(inventoryCategory + "가 꽉차서 넣을 수 없습니다.");
        }
    }

    // 리스트에 아이템을 넣는다. 아이템의 타입을 확인해서 세부 리스트에 추가한다 
    public void AddItem(Item item)
    {
        if (item == null) return;

        Debug.Log("TEST - 아이템 넣기 - " + item.itemName);

        // 아이템 타입 검사 장비라면 장비 타입별로 검사한다.
        if (item.itemType == ItemType.Equipment)
        {
            AddInventoryWithEquipment(item);
        }
        else
        {
            AddInventoryWithETC(item);
        }
    }

   
    public Item GetItemByUniqueID(int uniqueID)
    {
        foreach(var subList in itemList)
        {
            if (subList.Value == null) continue; 

            foreach(var item in subList.Value)
            {
                if (item == null) continue; 

                if(item.uniqueID == uniqueID)
                {
                    return item;
                }
            }
        }

        return null; 
    }

 

    public List<Item> GetItemList(InventoryCategory category)
    {
        if (itemList.ContainsKey(category) == true)
            return itemList[category];
        else
            return null;
    }

    // 장비 장착 
    public void EquipItemSlot(int _idx)
    {
        //SetApplyList(EquipMenu.instance.tabNumber);
      
    }


    // 재화 값 가져오기 
    public int GetMyCoinCount()
    {
        return itemList[InventoryCategory.ETC].Find(x => x.itemKeycode == "currency_coin").itemCount;
    }

    public void SetMyCoinCount(int value)
    {
        itemList[InventoryCategory.ETC].Find(x => x.itemKeycode == "currency_coin").itemCount += value;
    }
   
    public int GetMyAdventureCreditCount()
    {
        return itemList[InventoryCategory.ETC].Find(x => x.itemKeycode == "currency_adventure_credit").itemCount;
    }
}

