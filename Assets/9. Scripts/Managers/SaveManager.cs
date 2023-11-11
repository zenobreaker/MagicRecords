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
using Newtonsoft.Json;
using System.Xml;

[System.Serializable]
public class SaveData
{
    public float bgmSoundValue = 0.0f;
    public float sfxSoundValue = 0.0f;

    public string userID = "";
    public int money = 0;   // ������ ���� �ִ� �Ӵ�
    // �κ��丮
    public Dictionary<int, ItemData> inventory = new Dictionary<int, ItemData>();
    
    // ������ ������ �ִ� ĳ���͵� 
    public Dictionary<int, WheelerData> wheelerDatas = new Dictionary<int, WheelerData>();

    public bool gameInitAccess = false;   // ���� ���� �� ù �����ΰ�
    public int gameLevel = 0;   // ���� ���̵�
    public bool isAdventure = false; // Ž�� ����Ȯ��
    public bool initJoingFlag = false; // ���� ���������� �����ؾ��ϴ����� ���� �÷���
    public int currentChapter = 0;      // Ž�� �����ؾ��ϴ� é�� �� 
    public int currentStage = 0;     // Ž�� �����ؾ��ϴ� �������� ��

    public Dictionary<int, List<StageTableClass>> stageDictList = new Dictionary<int, List<StageTableClass>>();

    public bool choiceRecord = false; 
    // ������ Ž�縦 �����ϸ鼭 ���� ���ڵ��
    public List<int> recordList = new List<int>();

    public int version; 
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
    public List<SkillData> passvieSkills = new List<SkillData>();
    // todo ����? 

}

// ��ų ���� 
[System.Serializable]
public class SkillData
{
    public string keycode;
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

    public int uniqueID; 
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
    public int uniqueID;
   
    public EquipItemData()
    {
        equipType = EquipType.NONE;
        uniqueID = 0;
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

        Debug.Log("Save path: " + SAVE_DATA_DIRECTROTY);

        LoadData();
    }

    public void SaveData()
    {

        // ���� ���� 
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume* 10) / 10;

        // Ž�� ���൵ ����
        saveData.isAdventure = StageInfoManager.FLAG_ADVENTURE_MODE;
        saveData.initJoingFlag = StageInfoManager.initJoinPlayGameModeFlag;

        saveData.currentChapter = StageInfoManager.instance.currentChapter;
        saveData.stageDictList = StageInfoManager.instance.GetStageList();


        // ���ڵ� 
        saveData.choiceRecord = RecordManager.CHOICED_COMPLETE_RECORD;
        saveData.recordList.Clear();
        foreach (var record in RecordManager.instance.selectRecordInfos)
        {
            if (record == null) continue; 
            saveData.recordList.Add(record.id);
        }

        // ���� ���� ���� 
        SaveUserInfo();

       // dictionary ���� json���� �����ϱ� ����
        string json = JsonConvert.SerializeObject(saveData);

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
        saveData.money = InfoManager.coin;

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
        InfoManager.coin = saveData.money;
        
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
                        equipItemData.uniqueID = characters[i].equipItems[(EquipType)x].uniqueID;
                    }

                    wheeler.equipItems.Add(equipItemData);
                }

                // ��ų ���� 
                for (SkillSlotNumber j = SkillSlotNumber.SLOT1 ; j <= SkillSlotNumber.MAXSLOT; j++)
                {
                    SkillData skillData = new SkillData();
                    
                    if(characters[i].skills[j] != null)
                    {
                        skillData.keycode = characters[i].skills[j].keycode;
                        skillData.level = characters[i].skills[j].MySkillLevel;
                        skillData.chainSkill = characters[i].skills[j].isChain;
                    }

                    wheeler.skills.Add(skillData);
                }

                for (SkillSlotNumber k = SkillSlotNumber.CHAIN1 ; k <= SkillSlotNumber.MAXCHAINSLOT ; k++)
                {
                    SkillData skillData = new SkillData();

                    if(characters[i].chainsSkills[k] != null )
                    {
                        skillData.keycode = characters[i].chainsSkills[k].keycode;
                        skillData.level = characters[i].chainsSkills[k].MySkillLevel;
                        skillData.chainSkill = characters[i].chainsSkills[k].isChain;
                    }

                    wheeler.chainSkills.Add(skillData);
                }

                // �нú� ��ų 
                for (int idx = 0; idx < characters[i].equippedPassiveSkills.Count; idx++)
                {
                    if( characters[i].equippedPassiveSkills[idx] == null )
                    {
                        continue;
                    }

                    SkillData skillData = new SkillData();
                    skillData.keycode = characters[i].equippedPassiveSkills[idx].keycode;
                    skillData.level = characters[i].equippedPassiveSkills[idx].MySkillLevel;

                    wheeler.passvieSkills.Add(skillData);
                }

                
                if(saveData.wheelerDatas.ContainsKey(wheeler.wheelerID) == true)
                {
                    saveData.wheelerDatas[wheeler.wheelerID] = wheeler;
                }
                else
                {
                    saveData.wheelerDatas.Add(wheeler.wheelerID, wheeler);
                }
            }
        }
    }
    

    // ���ӿ� ������ ���� ���� �ٷ� 
    public void  ApplyWheelers()
    {
        foreach(var wheelerPair in saveData.wheelerDatas)
        {
            if (wheelerPair.Value == null) continue;
            // �ڽ��� ������ ĳ���ͷ� �߰� 
            var wheeler = wheelerPair.Value;
            CharStat charStat = PlayerDatabase.instance.GetCharStat(wheeler.wheelerID);
            charStat.level = wheeler.level;
            Character tempPlayer = new Character();
            tempPlayer.MyID = wheeler.wheelerID;
            tempPlayer.objectID = wheeler.wheelerID;
            // ���� 
            tempPlayer.MyStat = charStat;
            // ���� ���
            foreach(var equipItemData in wheeler.equipItems)
            {
                var equipItem =  Inventory.instance.GetItemByUniqueID(equipItemData.uniqueID);
                tempPlayer.EquipItem(equipItem as EquipItem);
            }

            // todo ���� ��ų  - id�� �����ؾ��ҵ�..
            //tempPlayer.equippedPassiveSkills.Add()
            
            
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
            if (itemList == null || itemList.Count <= 0) 
                continue; 

            for (int j = 0; j < itemList.Count; j++)
            {
                ItemData itemData = new ItemData();

                itemData.itemID = itemList[j].itemUID;
                itemData.count = itemList[j].itemCount;
                itemData.itemType = itemList[j].itemType;
                itemData.itemRank = itemList[j].itemRank;
                itemData.uniqueID = itemList[j].uniqueID;

                if (itemList[j] is EquipItem)
                {
                    EquipItem equipItem = itemList[j] as EquipItem;
                    itemData.userID = equipItem.userID;
                    itemData.equipType = equipItem.equipType;
                    itemData.enhanceCount = equipItem.itemEnchantRank;
                    itemData.itemMainAbility = equipItem.itemMainAbility;
                    itemData.itemAbilities = equipItem.itemAbilities;
                }

                if (saveData.inventory.ContainsKey(itemData.uniqueID) == true)
                {
                    saveData.inventory[itemData.uniqueID] = itemData;
                }
                else
                {
                    saveData.inventory.Add(itemData.uniqueID, itemData);
                }
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
            saveData = JsonConvert.DeserializeObject<SaveData>(data);
            
            LoadSoundData();

            // Ž�� ���൵ ����
            StageInfoManager.FLAG_ADVENTURE_MODE = saveData.isAdventure;
            StageInfoManager.initJoinPlayGameModeFlag = saveData.initJoingFlag;
            StageInfoManager.instance.currentChapter = saveData.currentChapter;
            StageInfoManager.instance.SetStageList(saveData.stageDictList);

            // ���ڵ� 
            RecordManager.CHOICED_COMPLETE_RECORD = saveData.choiceRecord;
            // ���ڵ� 
            foreach (var id in saveData.recordList)
            {
              RecordManager.instance.SelectRecord(id);
            }

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

        Debug.Log(saveData.sfxSoundValue + " , " + saveData.bgmSoundValue);
        for (int i = 0; i < SoundManager.instance.sfxPlayer.Length; i++)
        {
            SoundManager.instance.sfxPlayer[i].volume = saveData.sfxSoundValue;
        }

        SoundManager.instance.bgmPlayer.volume = (saveData.bgmSoundValue);

    }

 

    private void OnApplicationQuit()
    {
        //if (!SceneManager.GetActiveScene().name.Equals("Title"))
        SaveData();
    }
}
