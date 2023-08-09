using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InventoryCategory
{
    NONE = 0, 
    WEAPON = 1, 
    ARMOR,
    TIRE,
    ACCESORY, 
    ETC,
    DRONE,
    RUNE,
    TOTAL,
    MAX = 8,
}

// 유저의 아이템을 담아놓는 인벤토리 클래스
public class Inventory : MonoBehaviour
{
    public static Inventory instance; 

    public int maxSlotCount;        // 최대 인벤 슬롯 개수 

    //[SerializeField] List<Item> totalItems = new List<Item>();
    [SerializeField] List<Item> weaponItems = new List<Item>();
    [SerializeField] List<Item> armorItems = new List<Item>();
    [SerializeField] List<Item> tireItems = new List<Item>();
    [SerializeField] List<Item> accessoryItems = new List<Item>();
    [SerializeField] List<Item> usedItems = new List<Item>();
    [SerializeField] List<Item> etcItems = new List<Item>();
    [SerializeField] List<Item> droneItems = new List<Item>();
    [SerializeField] List<Item> runItems = new List<Item>();

    Dictionary<ItemType, List<Item>> itemList = new Dictionary<ItemType, List<Item>>();

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
    }

    void Start()
    {

        //TabSetting(0);
        //Debug.Log("탭세팅");
        // TabSlotOpen(selectedTab);


        // todo total item list 살려서 조작시키기 
        //itemList.Add(ItemType.Equipment, )
    }


    public List<Item> GetItemsByInventoryType(ItemType _type)
    {
        return itemList[_type];
    }
    
    // 아이템이 담긴 리스트 반환
    public List<Item> GetItems()
    {
        return weaponItems; // 기능 변경으로 주석 //totalItems;
    }

    public List<Item> GetItemByInventory(Item _item)
    {
        if (_item  == null)
        {
            return null; 
        }

        if (_item.itemType == ItemType.Equipment)
        {
            var equipItem = _item as EquipItem;
            if (equipItem != null)
            {
                if (equipItem.equipType == EquipType.WEAPON)
                {
                    return weaponItems;
                }
                else if (equipItem.equipType == EquipType.ARMOR)
                {
                    return armorItems;
                }
                else if (equipItem.equipType == EquipType.ACCSESORRY_1)
                {
                    return accessoryItems;
                }
                else if (equipItem.equipType == EquipType.WHEEL)
                {
                    return tireItems;
                }
                else if (equipItem.equipType == EquipType.DRONE)
                {
                    return droneItems;
                }
                else if (equipItem.equipType == EquipType.RUNE)
                {
                    return runItems;
                }

            }
        }
        // 소모품일 경우
        else if (_item.itemType == ItemType.Used)
        {
            return usedItems;
        }
        // 기타
        else if (_item.itemType == ItemType.ETC)
        {
            return etcItems;
        }

        return null;
    }

    // 리스트에 아이템을 넣는다. 아이템의 타입을 확인해서 세부 리스트에 추가한다 
    public void AddItem(Item _item)
    {
        // 기능 변경으로 주석 
        //totalItems.Add(_item);
        
        // 아이템 타입 검사 장비라면 장비 타입별로 검사한다.
        if(_item.itemType == ItemType.Equipment)
        {
            var equipItem = _item as EquipItem;
            if (equipItem != null)
            {
                if (equipItem.equipType == EquipType.WEAPON)
                {
                    weaponItems.Add(_item);
                }
                else if (equipItem.equipType == EquipType.ARMOR)
                {
                    armorItems.Add(_item);
                }
                else if (equipItem.equipType == EquipType.ACCSESORRY_1)
                {
                    accessoryItems.Add(_item);
                }
                else if (equipItem.equipType == EquipType.WHEEL)
                {
                    tireItems.Add(_item);
                }
                else if (equipItem.equipType == EquipType.DRONE)
                {
                    droneItems.Add(_item);
                }
                else if (equipItem.equipType == EquipType.RUNE)
                {
                    runItems.Add(_item);
                }

            }
        }
        // 소모품일 경우
        else if(_item.itemType == ItemType.Used)
        {
            usedItems.Add(_item);
        }
        // 기타
        else if(_item.itemType == ItemType.ETC)
        {
            etcItems.Add(_item);
        }

    }

    //public void SetInvenSlot(List<Item> p_itemList)
    //{
    //    for (int i = 0; i < p_itemList.Count; i++)
    //    {
    //        invenSlots[i].ClearSlot();
    //        invenSlots[i].SetSize(new Vector2(200, 200));
    //        invenSlots[i].AddItem(p_itemList[i]);
    //    }
    //}


    public List<Item> GetWeaponItems()
    {
        return weaponItems;
    }

    public List<Item> GetArmorItems()
    {
        return armorItems;
    }
    public List<Item> GetTireItems()
    {
        return tireItems;
    }
    public List<Item> GetAccessoryItems()
    {
        return accessoryItems;
    }

    public List<Item> GetUsedItems()
    {
        return usedItems;
    }

    public List<Item> GetDroneItems()
    {
        return droneItems;
    }

    public List<Item> GetRunItems()
    {
        return runItems;
    }

    public List<Item> GetItemList(InventoryCategory _category)
    {
        switch(_category )
        {
            case InventoryCategory.WEAPON:
                return GetWeaponItems();
            case InventoryCategory.ARMOR:
                return GetArmorItems();
            case InventoryCategory.TIRE:
                return GetTireItems(); 
            case InventoryCategory.ACCESORY:
                return GetAccessoryItems(); 
            case InventoryCategory.ETC:
                return GetUsedItems(); 
            case InventoryCategory.DRONE:
                return GetDroneItems();
            case InventoryCategory.RUNE:
                return GetRunItems() ;
        }

        return GetWeaponItems(); 
    }

    // 장비 장착 
    public void EquipItemSlot(int _idx)
    {
        //SetApplyList(EquipMenu.instance.tabNumber);
      
    }
   
    // 저장관련 
    #region save 

    // 저장하기위한 데이터 반환 
    // 모든 아이템 

    // 무기
    public Item[] GetSaveWeaponData()
    {
        return weaponItems.ToArray();
    }

    // 방어구  
    public Item[] GetSaveArmorData()
    {
        return armorItems.ToArray();
    }


    // 타이어  
    public Item[] GetSaveTireData()
    {
        return tireItems.ToArray();
    }


    // 악세사리 
    public Item[] GetSaveAccessoryItemData()
    {
        return accessoryItems.ToArray();
    }

    // 소모품 
    public Item[] GetSaveUsedItemData()
    {
        return usedItems.ToArray();
    }

    // 기타 아이템  
    public Item[] GetSaveETCItemData()
    {
        return etcItems.ToArray();
    }



#endregion

}

