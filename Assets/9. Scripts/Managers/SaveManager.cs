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
    public int money = 0;   // 유저가 갖고 있는 머니
    // 인벤토리
    public Dictionary<int, ItemData> inventory = new Dictionary<int, ItemData>();
    
    // 유저가 가지고 있는 캐릭터들 
    public Dictionary<int, WheelerData> wheelerDatas = new Dictionary<int, WheelerData>();

    public bool gameInitAccess = false;   // 게임 종료 후 첫 접속인가
    public int gameLevel = 0;   // 게임 난이도
    public bool isAdventure = false; // 탐사 진행확인
    public bool initJoingFlag = false; // 다음 스테이지를 진행해야하는지에 대한 플래그
    public int currentChapter = 0;      // 탐사 진행해야하는 챕터 수 
    public int currentStage = 0;     // 탐사 진행해야하는 스테이지 수

    public Dictionary<int, List<StageTableClass>> stageDictList = new Dictionary<int, List<StageTableClass>>();

    public bool choiceRecord = false; 
    // 유저가 탐사를 진행하면서 얻은 레코드들
    public List<int> recordList = new List<int>();

    public int version; 
}


// 플레이어블 캐릭터 저장 
[System.Serializable]
public class WheelerData
{
    public int saveDataID = 0;  // 이 데이터를 가진 data 주인의 ID
    public int wheelerID = 0;   // 대상 캐릭터 ID
    //캐릭터의 스탯들 
    public int level = 1;
    // 기타 스탯은 캐릭터를 생성할 때 레벨을 기준으로 계산해서 처리한다. 
    // 계수랑은 캐릭터 ID로 찾아서 캐릭터마다 따로 처리한다. 
    // todo 캐릭터마다 성장 스탯이 다르다면 그 성장 스탯을 저장하는 파일을 처리해야한다. 

    public int exp = 0;
    public int maxExp = 100;

    // 엑스트라 스탯은 장착한 장비나 스킬 등에 대한 외부 요인으로 처리되므로 저장안함
    // 장착한 장비 id만 저장하고 인벤토리에서 해당 id에 아이템을 가져와서 장착시킨다
    public List<EquipItemData> equipItems = new List<EquipItemData>();

    // 장착한 스킬 
    public List<SkillData> skills = new List<SkillData>();
    public List<SkillData> chainSkills = new List<SkillData>();
    public List<SkillData> passvieSkills = new List<SkillData>();
    // todo 유물? 

}

// 스킬 정보 
[System.Serializable]
public class SkillData
{
    public string keycode;
    public int level;
    public bool chainSkill;
}

// 아이템 정보
[System.Serializable]
public class ItemData
{
    public int itemID;
    public ItemType itemType;
    public ItemRank itemRank;
    public int count;

    public int uniqueID; 
    public int userID;      // 소유자 
    public EquipType equipType;
    public int enhanceCount;    // 강화수치 
    public ItemAbility itemMainAbility; // 아이템 능력 수치 
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

        // 사운드 저장 
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume* 10) / 10;

        // 탐사 진행도 관련
        saveData.isAdventure = StageInfoManager.FLAG_ADVENTURE_MODE;
        saveData.initJoingFlag = StageInfoManager.initJoinPlayGameModeFlag;

        saveData.currentChapter = StageInfoManager.instance.currentChapter;
        saveData.stageDictList = StageInfoManager.instance.GetStageList();


        // 레코드 
        saveData.choiceRecord = RecordManager.CHOICED_COMPLETE_RECORD;
        saveData.recordList.Clear();
        foreach (var record in RecordManager.instance.selectRecordInfos)
        {
            if (record == null) continue; 
            saveData.recordList.Add(record.id);
        }

        // 유저 정보 저장 
        SaveUserInfo();

       // dictionary 값을 json으로 저장하기 위함
        string json = JsonConvert.SerializeObject(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
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

    // 플레이어 유저 정보 저장
    public void SaveUserInfo()
    {
        // 플레이한 유저 데이터 저장 
        saveData.money = InfoManager.coin;

        if (isExistFile == true)
        {
            // 유저 아이디값 설정해주기 
            saveData.userID = GenerateRandomString(8);
        }

        SaveWheelers();

        SaveInventory(); 
    }

    // 플레이어 유저 정보 적용

    public void ApplyUserInfo()
    {
        // 저장한 데이터가 있으면 적용하기 
        Debug.Log("저장된 파일 확인 완료");
        // 게임머니 
        InfoManager.coin = saveData.money;
        
        ApplyInvetory();
        
        ApplyWheelers();
    }


    public void SaveWheelers()
    {

        // 가지고 있는 캐릭터들 저장
        List<Character> characters = InfoManager.instance.GetMyPlayerInfoList();
        if (characters.Count > 0)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                WheelerData wheeler = new WheelerData();
                wheeler.wheelerID = characters[i].MyID;
                wheeler.exp = characters[i].MyStat.exp;
                wheeler.maxExp = characters[i].MyStat.maxExp;
          
                // 장착한 장비 정보
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

                // 스킬 정보 
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

                // 패시브 스킬 
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
    

    // 게임에 저장한 정보 세팅 휠러 
    public void  ApplyWheelers()
    {
        foreach(var wheelerPair in saveData.wheelerDatas)
        {
            if (wheelerPair.Value == null) continue;
            // 자신이 소유한 캐릭터로 추가 
            var wheeler = wheelerPair.Value;
            CharStat charStat = PlayerDatabase.instance.GetCharStat(wheeler.wheelerID);
            charStat.level = wheeler.level;
            Character tempPlayer = new Character();
            tempPlayer.MyID = wheeler.wheelerID;
            tempPlayer.objectID = wheeler.wheelerID;
            // 스탯 
            tempPlayer.MyStat = charStat;
            // 장착 장비
            foreach(var equipItemData in wheeler.equipItems)
            {
                var equipItem =  Inventory.instance.GetItemByUniqueID(equipItemData.uniqueID);
                tempPlayer.EquipItem(equipItem as EquipItem);
            }

            // todo 장착 스킬  - id로 생각해야할듯..
            //tempPlayer.equippedPassiveSkills.Add()
            
            
            // ? 유무울?

            // 캐릭터 정보 저장 
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

    // 게임에 저장한 정보 세팅 인벤토리
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
        // 선택한 saveCount 값에 대응하는 파일이 없다면 새롭게 시작하는 것 
        // 값이 있는 경우 
        if (File.Exists(SAVE_DATA_DIRECTROTY + SAVE_FILENAME))
        {
            isExistFile = true;
            string data = File.ReadAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME);
            // 파일에서 세이브 파일을 가져와 세팅한다. 
            saveData = JsonConvert.DeserializeObject<SaveData>(data);
            
            LoadSoundData();

            // 탐사 진행도 관련
            StageInfoManager.FLAG_ADVENTURE_MODE = saveData.isAdventure;
            StageInfoManager.initJoinPlayGameModeFlag = saveData.initJoingFlag;
            StageInfoManager.instance.currentChapter = saveData.currentChapter;
            StageInfoManager.instance.SetStageList(saveData.stageDictList);

            // 레코드 
            RecordManager.CHOICED_COMPLETE_RECORD = saveData.choiceRecord;
            // 레코드 
            foreach (var id in saveData.recordList)
            {
              RecordManager.instance.SelectRecord(id);
            }

            // 유저 정보 세팅
            ApplyUserInfo();

        }
        // 값이 없는 경우 
        else
        {
            isExistFile = false;
            // 없으면 새로 시작 
            Debug.Log("Don't have a save file");

            // todo 나중에 해금관련해서 캐릭터를 얻으면 풀려지도록 수정해보자 지금은 남생이만 넣는다.
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
