using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Item;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Random = UnityEngine.Random;


[System.Serializable]
public class MaterialJsonData
{
    public int id;
    public string name;
    public string description;
    public string imagePath;
}

[System.Serializable]
public class MaterialJsonAllData
{
    public MaterialJsonData[] materialJsonData;
}


public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    public TextAsset items;
    public TextAsset subOptions;
    public TextAsset materialJson;

    public List<Item> weaponList;
    public List<Item> armorList;
    public List<Item> wheelList;
    public List<Item> accessroyList;
    public List<Item> itemList;


    public MaterialJsonAllData materialJsonAllData;

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

        //InitializeItemList();
    }


    // 재화 
    public void InitializeMaterialJson()
    {
        if (materialJson == null) return; 

        materialJsonAllData = JsonUtility.FromJson<MaterialJsonAllData>(materialJson.text);
        if (materialJsonAllData.materialJsonData == null) return; 

        foreach(var material in materialJsonAllData.materialJsonData)
        {
            //Item item = new Item(material.id, material.name, material.name, ItemType.NONE, ItemRank.NONE, material.description,
            //      0, 0, material.imagePath);
            Item item = new Item(material.id, material.name, material.description);
            item.SetItemImageForFullPath("Resources/" + material.imagePath);

            if(item != null)
            {
                Debug.Log(item.itemName);
            }

            itemDataList.Add(item.itemUID, item);
        }

    }

    public void InitializeItemList()
    {
        if (items == null)
        {
            return;
        }
        string[] line = items.text.Substring(0, items.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
            // todo 아이템 값 데이터 변경되었으니 그에 걸맞게 조정해야됨
            // 앞뒤 공백 제거 
            //line[i].Replace("\r", "");
            string[] row = line[i].Split('\t');
            if (row.Length < 23) continue;

            int itemUID = int.Parse(row[0]);
            string keycode = row[1];
            string itemName = row[2];
            ItemType itemType = DiscernToItemType(row[3]);
            ItemRank itemRank = DiscernToItemRank(row[4]);
            int itemEach = int.Parse(row[5]);
            int itemValue = int.Parse(row[6]);
            string itemDesc = row[7].Replace("\\n", "\n");
            string itemImgID = row[8];
            bool isSale = row[9].Equals("TRUE");     // 상점에 판매할 여부 
            EquipType equipType = DiscernToEquipType(row[10]);
            int enhance = int.Parse(row[11]);
            int itemMainAbilType = int.Parse(row[12]);
            bool isMainPercent = row[13].Equals("TRUE");
            int itemMainAbility = int.Parse(row[14]);

            int itemSubAbilType1 = int.Parse(row[15]);
            bool isPercent1 = row[16].Equals("TRUE");
            int itemSubAbility1 = int.Parse(row[17]);

            int itemSubAbilType2 = int.Parse(row[18]);
            bool isPercent2 = row[19].Equals("TRUE");
            int itemSubAbility2 = int.Parse(row[20]);

            int itemSubAbilType3 = int.Parse(row[21]);
            bool isPercent3 = row[22].Equals("TRUE");
            row[23] = row[23].TrimEnd();
            int number = 0;
            int itemSubAbility3 = 0;
            if (int.TryParse(row[23], out number) == true)
            {
                itemSubAbility3 = number;
            }

            // 아이템 형태가 장비형 아이템일 경우 
            if (itemType == ItemType.Equipment)
            {
                //ItemAbility[] itemAbilities = SplitSubItemOption(itemUID);
                // 주능력 생성 
                ItemAbility mainAbility = new ItemAbility();
                mainAbility.abilityType = (AbilityType)itemMainAbilType;
                mainAbility.isPercent = isMainPercent;
                mainAbility.power = itemMainAbility;

                EquipItem equipItem = new EquipItem(itemUID, keycode, itemName, itemType,
                   itemRank, itemDesc, itemEach, itemValue, itemImgID,
                   equipType, enhance, false, mainAbility, false);

                // 보조 능력 생성 
                ItemAbility[] subAbilities = new ItemAbility[3];

                subAbilities[0].abilityType = (AbilityType)itemSubAbilType1;
                subAbilities[0].isPercent = isPercent1;
                subAbilities[0].power = itemSubAbility1;

                subAbilities[1].abilityType = (AbilityType)itemSubAbilType2;
                subAbilities[1].isPercent = isPercent2;
                subAbilities[1].power = itemSubAbility2;

                subAbilities[2].abilityType = (AbilityType)itemSubAbilType3;
                subAbilities[2].isPercent = isPercent3;
                subAbilities[2].power = itemSubAbility3;

                equipItem.SetSubAbility(subAbilities);


                itemDataList.Add(i, equipItem);

                if (equipType == EquipType.WEAPON)
                {
                    weaponList.Add(equipItem);
                }
                else if (equipType == EquipType.ARMOR)
                {
                    armorList.Add(equipItem);
                }
                else if (equipType == EquipType.ACCSESORRY_1 
                    || equipType == EquipType.ACCSESORRY_2
                    || equipType == EquipType.ACCSESORRY_3)
                {
                    accessroyList.Add(equipItem);
                }
                else if (equipType == EquipType.WHEEL)
                {
                    wheelList.Add(equipItem);
                }
            }
            else
            {
                // 아이템 생성 
                Item t_Item = new Item(itemUID, keycode, itemName, itemType, itemRank, itemDesc,
                    itemEach, itemValue, itemImgID, isSale);

                itemDataList.Add(i, t_Item);
                itemList.Add(t_Item);
            }
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
                return  ItemRank.Common;
            case "Magic":
                return  ItemRank.Magic;
            case "Rare":
                return  ItemRank.Rare;
            case "Unique":
                return  ItemRank.Unique;
            case "Legendary":
                return  ItemRank.Legendary;
            default:
                return  ItemRank.NONE;
        }
    }

    // 아이템 생성 
    public Item GetItem(string _keycode)
    {
        //string[] line = items.text.Substring(0, items.text.Length - 1).Split('\n');
        
        foreach(var pair in itemDataList)
        {
            if(pair.Value.itemKeycode.Equals(_keycode))
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
        foreach(var pair in itemDataList)
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
        while(InventoryContains(random))
        {
            random = Random.Range(min, max);
            item.uniqueID = random;
        }
    }

    public bool InventoryContains(int id)
    {

        foreach(var itemPair in Inventory.instance.itemList)
        {
            if (itemPair.Value == null) continue;
            foreach(var item in itemPair.Value)
            {
                if (item== null) continue;

                if(item.uniqueID == id)
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

