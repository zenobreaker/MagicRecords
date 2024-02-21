using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Item;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Random = UnityEngine.Random;


[System.Serializable] 
public class JsonData
{
    public int id;
}


[System.Serializable]
public class MaterialJsonData : JsonData
{
    public string name;
    public string description;
    public string imagePath;
}

[System.Serializable]
public class MaterialJsonAllData
{
    public MaterialJsonData[] materialJsonData;
}

[System.Serializable]
public class EquipmentJsonData : JsonData
{
    public string keycode;
    public int equipType;
    public int rank;
    public string name;
    public string description;
    public string imagePath;
    public int abilityType;
    public int abilityValue;
    public int isPerscent;
    public int itemValue;

}


[System.Serializable]
public class EquipmentJsonAllData
{
    public EquipmentJsonData[] equipmentJsonData;
}




public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    public TextAsset items;
    public TextAsset subOptions;

    public TextAsset materialJson;
    public TextAsset equipmentJson;


    public List<Item> weaponList;
    public List<Item> armorList;
    public List<Item> wheelList;
    public List<Item> accessroyList;
    public List<Item> itemList;


    public MaterialJsonAllData materialJsonAllData;
    public EquipmentJsonAllData equipmentJsonAllData;

    [SerializeField]
    Dictionary<int, Item> itemDataList;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        itemDataList = new Dictionary<int, Item>();


        InitializeMaterialJson();

        InitializeEquipItemJson();
    }


    // 재화 
    public void InitializeMaterialJson()
    {
        if (materialJson == null) return;

        materialJsonAllData = JsonUtility.FromJson<MaterialJsonAllData>(materialJson.text);
        if (materialJsonAllData.materialJsonData == null) return;

        foreach (var material in materialJsonAllData.materialJsonData)
        {
            Item item = new Item(material.id, material.name, material.description, material.imagePath);
            //item.SetItemImageForFullPath("Image/" + material.imagePath);
            item.itemType = ItemType.ETC;
            if (item != null)
            {
                Debug.Log(item.itemName);
            }

            itemDataList.Add(item.itemUID, item);
        }

    }

    public void InitializeEquipItemJson()
    {
        if (equipmentJson == null) return;

        equipmentJsonAllData = JsonUtility.FromJson<EquipmentJsonAllData>(equipmentJson.text);
        if (equipmentJsonAllData.equipmentJsonData == null) return;

        foreach (var json in equipmentJsonAllData.equipmentJsonData)
        {
            ItemAbility itemAbility = new ItemAbility();
            itemAbility.abilityType = (AbilityType)json.abilityType;
            itemAbility.power = json.abilityValue;
            itemAbility.isPercent = json.isPerscent == 1 ? true : false;

            EquipItem equipItem = new EquipItem(json.id, json.keycode, json.name,
                ItemType.Equipment, (ItemRank)json.rank, json.description, 1,
                json.itemValue, json.imagePath, (EquipType)json.equipType, 0,
                false, itemAbility);

            itemDataList.Add(equipItem.itemUID, equipItem);
        }

    }
    // 아이템 정보 파일이 설명을 가진 것부터 값을 가진 파일로 나눠져 있다 값을 가진 파일에서 해당 아이템의 
    // 능력치 타입과 밸류 값을 알아낸다

    // 보조 능력치 배열을 만들어 반환한다
    ItemAbility[] CreateSubAbility()
    {
        return null;
    }


    // 대상 아이템의 서브 옵션을 관련 파일에서 찾아서 옵션 리스트를 반환한다. 
    ItemAbility[] SplitSubItemOption(string _uid)
    {
        string[] line = subOptions.text.Substring(0, subOptions.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');
            string itemUID = row[0];

            if (_uid == itemUID)
            {
                ItemAbility[] _itemAbility = new ItemAbility[3];

                for (int j = 0; j < _itemAbility.Length; j++)
                {
                    // _itemAbility[j].type = row[j * 3 + 1];
                    // _itemAbility[j].isPercent = row[j * 3 + 2] == "TRUE";
                    _itemAbility[j].power = int.Parse(row[j * 3 + 3]);
                }

                return _itemAbility;
            }
        }

        return null;
    }

    ItemType DiscernToItemType(string _itemType)
    {
        switch (_itemType)
        {
            case "Equipment":
                return ItemType.Equipment;
            case "Used":
                return ItemType.Used;
            case "Ingredient":
                return ItemType.Ingredient;
            case "ETC":
                return ItemType.ETC;
            default:
                return ItemType.Coin;
        }
    }

    EquipType DiscernToEquipType(string _equipType)
    {
        switch (_equipType)
        {
            case "Weapon":
                return EquipType.WEAPON;
            case "Armor":
                return EquipType.ARMOR;
            case "Accessory":
                return EquipType.ACCSESORRY_1;
            default:
                return EquipType.NONE;
        }
    }

    ItemRank DiscernToItemRank(string _itemRank)
    {
        switch (_itemRank)
        {
            case "Common":
                return ItemRank.Common;
            case "Magic":
                return ItemRank.Magic;
            case "Rare":
                return ItemRank.Rare;
            case "Unique":
                return ItemRank.Unique;
            case "Legendary":
                return ItemRank.Legendary;
            default:
                return ItemRank.NONE;
        }
    }

    // 아이템 생성 
    public Item GetItem(string _keycode)
    {
        //string[] line = items.text.Substring(0, items.text.Length - 1).Split('\n');

        foreach (var pair in itemDataList)
        {
            if (pair.Value.itemKeycode.Equals(_keycode))
            {
                Item item = (Item)pair.Value.Clone();
                CreateUniqueID(item);
                return item;
            }
        }

        return null;
    }

    public Item GetItemByUID(int uid)
    {
        foreach (var pair in itemDataList)
        {
            if (pair.Value.itemUID == uid)
            {
                Item item = (Item)pair.Value.Clone();
                CreateUniqueID(item);
                return item;
            }
        }

        return null;
    }

    public void CreateUniqueID(Item item)
    {
        if (item == null) return;

        int min = 1;
        int max = 99999999;

        int random = Random.Range(min, max);
        item.uniqueID = random;
        while (InventoryContains(random))
        {
            random = Random.Range(min, max);
            item.uniqueID = random;
        }
    }

    public bool InventoryContains(int id)
    {

        foreach (var itemPair in Inventory.instance.itemList)
        {
            if (itemPair.Value == null) continue;
            foreach (var item in itemPair.Value)
            {
                if (item == null) continue;

                if (item.uniqueID == id)
                {
                    return true;
                }
            }
        }

        return false;
    }


    // 무기 아이템 만들기
    void CreateWeaponItem()
    {

    }

    // 무기 리스트에 해당 아이템 넣기 
    void SetWeaponItemToList(Item _item)
    {
        weaponList.Add((EquipItem)_item);
    }


    // 무기 리스트를 반환 
    List<Item> GetWeaponItemList()
    {
        return null;
    }

}


