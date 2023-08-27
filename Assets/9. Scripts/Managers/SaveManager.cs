using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using UnityEditor;
using System;
using System.Linq;
using Random = UnityEngine.Random;

[System.Serializable]
public class SaveData
{
    public float bgmSoundValue = 0.0f;
    public float sfxSoundValue = 0.0f;

    public string userID = "";
    public int money = 0;   // ������ ���� �ִ� �Ӵ�
    // �κ��丮
    public List<ItemData> inventory = new List<ItemData>();
    
    // ������ ������ �ִ� ĳ���͵� 
    public List<WheelerData> wheelerDatas = new List<WheelerData>();
}

// �÷��̾�� ĳ���� ���� 
[System.Serializable]
public class WheelerData
{
    public int saveDataID = 0;  // �� �����͸� ���� data ������ ID
    public int wheelerID = 0;   // ��� ĳ���� ID
    //ĳ������ ���ȵ� 
    public int level = 1;
    // ��Ÿ ������ ĳ���͸� ������ �� ������ �������� ����ؼ� ó���Ѵ�. 
    // ������� ĳ���� ID�� ã�Ƽ� ĳ���͸��� ���� ó���Ѵ�. 
    // todo ĳ���͸��� ���� ������ �ٸ��ٸ� �� ���� ������ �����ϴ� ������ ó���ؾ��Ѵ�. 

    public int exp = 0;
    public int maxExp = 100;

    // ����Ʈ�� ������ ������ ��� ��ų � ���� �ܺ� �������� ó���ǹǷ� �������
    // ������ ��� id�� �����ϰ� �κ��丮���� �ش� id�� �������� �����ͼ� ������Ų��
    public List<EquipItemData> equipItems = new List<EquipItemData>();

    // ������ ��ų 
    public List<SkillData> skills = new List<SkillData>();
    public List<SkillData> chainSkills = new List<SkillData>();

    // todo ����? 

}

// ��ų ���� 
[System.Serializable]
public class SkillData
{
    public string name;
    public int level;
    public bool chainSkill;
}

// ������ ����
[System.Serializable]
public class ItemData
{
    public int itemID;
    public ItemType itemType;
    public ItemRank itemRank;
    public int count;

    public int userID;      // ������ 
    public EquipType equipType;
    public int enhanceCount;    // ��ȭ��ġ 
    public ItemAbility itemMainAbility; // ������ �ɷ� ��ġ 
    public ItemAbility[] itemAbilities;

    public ItemData()
    {
        itemID = 0;
        itemType = ItemType.NONE;
        count = 0;

        equipType = EquipType.NONE;
        userID = 0;
        enhanceCount = 0;
        itemMainAbility.abilityType = AbilityType.NONE;
        itemMainAbility.power = 0;
        itemMainAbility.isPercent = false;
    }
}

[System.Serializable]
public class EquipItemData
{
    public EquipType equipType;
    public int equipItemID;
   
    public EquipItemData()
    {
        equipType = EquipType.NONE;
        equipItemID = 0;
    }

}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;

    [SerializeField] GameObject go_BackGround = null;

    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTROTY;
    private string SAVE_FILENAME = "/SaveFile";
    private string INFO_SAVE_FILENAME = "/InfoSaveFile";
    private string INFO_HAVE_SKILL_SAVE_FILENAME = "/HaveSkillSaveFile";
    private string INFO_ITEM_SAVE_FILENAME = "/HaveITEMSaveFile";
    private bool isExistFile = false; 

    [SerializeField] private Inventory theInven;
    [SerializeField] InteractionController theIC = null;
    [SerializeField] SoundManager soundManager = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SAVE_DATA_DIRECTROTY = Application.persistentDataPath + "/Saves/";
        //Debug.Log(SAVE_DATA_DIRECTROTY);

        if (!Directory.Exists(SAVE_DATA_DIRECTROTY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTROTY);

        LoadData();
    }

    public void SaveData()
    {

        // ���� ���� 
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume* 10) / 10;


        // ���� ���� ���� 
        SaveUserInfo();

       
        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME, json);

        Debug.Log("���� �Ϸ�");
        Debug.Log(json);
    }

    string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string randomString = "";

        for (int i = 0; i < length; i++)
        {
            int randomIndex = Random.Range(0, chars.Length);
            randomString += chars[randomIndex];
        }

        return randomString;
    }

    // �÷��̾� ���� ���� ����
    public void SaveUserInfo()
    {
        // �÷����� ���� ������ ���� 
        saveData.money = InfoManager.instance.money;

        if (isExistFile == true)
        {
            // ���� ���̵� �������ֱ� 
            saveData.userID = GenerateRandomString(8);
        }

        SaveWheelers();

        SaveInventory(); 
    }

    // �÷��̾� ���� ���� ����

    public void ApplyUserInfo()
    {
        // ������ �����Ͱ� ������ �����ϱ� 
        Debug.Log("����� ���� Ȯ�� �Ϸ�");
        // ���ӸӴ� 
        InfoManager.instance.money = saveData.money;
        
        ApplyInvetory();
        
        ApplyWheelers();
    }


    public void SaveWheelers()
    {

        // ������ �ִ� ĳ���͵� ����
        List<Character> characters = InfoManager.instance.GetMyPlayerInfoList();
        if (characters.Count > 0)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                WheelerData wheeler = new WheelerData();
                wheeler.wheelerID = characters[i].MyID;
                wheeler.exp = characters[i].MyStat.exp;
                wheeler.maxExp = characters[i].MyStat.maxExp;
          
                // ������ ��� ����
                for (int x = 1; x <= 7; x++)
                {
                    EquipItemData equipItemData = new EquipItemData();
                    equipItemData.equipType = (EquipType)x;
                    if (characters[i].equipItems[(EquipType)x] != null)
                    {
                        equipItemData.equipItemID = characters[i].equipItems[(EquipType)x].itemUID;
                    }

                    wheeler.equipItems.Add(equipItemData);
                }

                // ��ų ���� 
                for (int j = 0; j < characters[i].MySkills.Length; j++)
                {
                    SkillData skillData = new SkillData();
                    
                    if(characters[i].MySkills[j] != null)
                    {
                        skillData.name = characters[i].MySkills[j].MyName;
                        skillData.level = characters[i].MySkills[j].MySkillLevel;
                        skillData.chainSkill = characters[i].MySkills[j].isChain;
                    }

                    wheeler.skills.Add(skillData);
                }

                for (int k = 0; k < characters[i].MyChains.Length; k++)
                {
                    SkillData skillData = new SkillData();

                    if(characters[i].MyChains[k] != null )
                    {
                        skillData.name = characters[i].MyChains[k].MyName;
                        skillData.level = characters[i].MyChains[k].MySkillLevel;
                        skillData.chainSkill = characters[i].MyChains[k].isChain;
                    }

                    wheeler.chainSkills.Add(skillData);
                }

                saveData.wheelerDatas.Add(wheeler);
            }
        }
    }
    

    // ���ӿ� ������ ���� ���� �ٷ� 
    public void  ApplyWheelers()
    {
        foreach(var wheeler in saveData.wheelerDatas)
        {
            if (wheeler == null) continue;
            // �ڽ��� ������ ĳ���ͷ� �߰� 
            CharStat charStat = MonsterDatabase.instance.GetCharStat(wheeler.wheelerID);
            charStat.level = wheeler.level;
            Character tempPlayer = new Character();
            tempPlayer.MyID = wheeler.wheelerID;
            tempPlayer.objectID = (uint)wheeler.wheelerID;
            // ���� 
            tempPlayer.MyStat = charStat;
            // ���� ���
            foreach(var equipItemData in wheeler.equipItems)
            {
                var equipItem =  Inventory.instance.GetItemByID(equipItemData.equipItemID);
                tempPlayer.EquipItem(equipItem as EquipItem);
            }

            // ���� ��ų  - id�� �����ؾ��ҵ�..
            //tempPlayer.SetSkill()
            // ? ������?

            // ĳ���� ���� ���� 
            InfoManager.instance.AddMyPlayerInfo(wheeler.wheelerID, tempPlayer);
        }
    }

    public void SaveInventory()
    {

        for(int i = 0; i < (int)InventoryCategory.MAX; i++)
        {
            if (Inventory.instance.itemList[(InventoryCategory)i] == null)
            {
                continue; 
            }

            var itemList = Inventory.instance.itemList[(InventoryCategory)i];
            for (int j = 0; j < itemList.Count; j++)
            {
                ItemData itemData = new ItemData();
                if (itemList[i] == null) continue; 

                itemData.itemID = itemList[i].itemUID;
                itemData.count = itemList[i].itemCount;
                itemData.itemType = itemList[i].itemType;
                itemData.itemRank = itemList[i].itemRank;

                if (itemList[i] is EquipItem)
                {
                    EquipItem equipItem = itemList[i] as EquipItem;
                    itemData.userID = equipItem.userID;
                    itemData.equipType = equipItem.equipType;
                    itemData.enhanceCount = equipItem.itemEnchantRank;
                    itemData.itemMainAbility = equipItem.itemMainAbility;
                    itemData.itemAbilities = equipItem.itemAbilities;
                }
                
                saveData.inventory.Add(itemData);
            }
        }

    }

    // ���ӿ� ������ ���� ���� �κ��丮
    public void ApplyInvetory()
    {
        InventoryManager.instance.ApplySaveItemData(saveData.inventory);
    }

 
    

    public void SaveSoundData()
    {
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume * 10) / 10;
    }

    public void LoadData()
    {
        // ������ saveCount ���� �����ϴ� ������ ���ٸ� ���Ӱ� �����ϴ� �� 
        // ���� �ִ� ��� 
        if (File.Exists(SAVE_DATA_DIRECTROTY + SAVE_FILENAME))
        {
            isExistFile = true;
            string data = File.ReadAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME);
            // ���Ͽ��� ���̺� ������ ������ �����Ѵ�. 
            saveData = JsonUtility.FromJson<SaveData>(data);
            
            LoadSoundData();
            // ���� ���� ����
            ApplyUserInfo();

        }
        // ���� ���� ��� 
        else
        {
            isExistFile = false;
            // ������ ���� ���� 
            Debug.Log("Don't have a save file");

            // todo ���߿� �رݰ����ؼ� ĳ���͸� ������ Ǯ�������� �����غ��� ������ �����̸� �ִ´�.
            InfoManager.instance.AddMyPlayerInfo(1);
        }
    }       

    void LoadSoundData()
    {
        if (File.Exists(SAVE_DATA_DIRECTROTY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            Debug.Log(saveData.sfxSoundValue + " , " + saveData.bgmSoundValue);
            for (int i = 0; i < SoundManager.instance.sfxPlayer.Length; i++)
            {
                SoundManager.instance.sfxPlayer[i].volume = saveData.sfxSoundValue;
            }

            SoundManager.instance.bgmPlayer.volume = (saveData.bgmSoundValue);
        }
    }

 

    private void OnApplicationQuit()
    {
        //if (!SceneManager.GetActiveScene().name.Equals("Title"))
        SaveData();
    }
}
