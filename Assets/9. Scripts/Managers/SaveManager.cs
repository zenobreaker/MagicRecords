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
    public int money = 0;   // 유저가 가지고 있는 재화 
    // 아이템 인벤토리 
    public Dictionary<int, ItemData> inventory = new Dictionary<int, ItemData>();
    
    // 소유한 휠러(캐릭터) 데이터들 
    public Dictionary<int, WheelerData> wheelerDatas = new Dictionary<int, WheelerData>();

    public bool gameInitAccess = false;   // 게임 (탐사) 진입 여부 플래그값 
    public int gameLevel = 0;   // 진행한 난이도 
    public bool isAdventure = false; // 탐사 상태인지 
    public bool initJoinFlag = false; // 탐사를 시작한 상태인지 
    public int currentChapter = 0;      // 현재 진행 중인 챕터 
    public int currentStage = 0;     // 현재 진행 중인 스테이지 

    public Dictionary<int, List<StageNodeInfo>> stageDictList = new Dictionary<int, List<StageNodeInfo>>();

    public bool choiceRecord = false; 
    // 진행하면서 얻은 레코드 리스트 
    public List<int> recordList = new List<int>();

    // 저장 버전
    public int version; 
}


// 휠러(캐릭터) 데이터 정보 클래스
[System.Serializable]
public class WheelerData
{
    public int saveDataID = 0;  // data ID
    public int wheelerID = 0;   // ID

    // 휠러의 레벨 
    public int level = 1;

    public int exp = 0;
    public int maxExp = 100;

    // 휠러가 장착한 아이템의 정보
    public List<EquipItemData> equipItems = new List<EquipItemData>();

    // 휠러가 배운 스킬 저보 
    public List<SkillData> skills = new List<SkillData>();
    public List<SkillData> chainSkills = new List<SkillData>();
    public List<SkillData> passvieSkills = new List<SkillData>();
    // todo ����? 
}

// 스킬 정보 클래스
[System.Serializable]
public class SkillData
{
    public string keycode;
    public int level;
    public bool chainSkill;
}

// 스킬 전체를 저장을 담당하는 클래스 
[System.Serializable]
public class SkillSaveData
{
    public string userID = ""; // 스킬 정보의 주인 ID
    
    public List<SkillData> acitveSkillList = new List<SkillData>();
    public List<SkillData> passiveSkillList = new List<SkillData>();

    public int version; // 저장 버전 
}


// 아이템 정보 클래스
[System.Serializable]
public class ItemData
{
    public int itemID;
    public ItemType itemType;
    public ItemRank itemRank;
    public int count;

    public int uniqueID; 
    public int userID;      // 아이템을 사용하는 대상 ID 
    public EquipType equipType;
    public int enhanceCount;    // 강화 수치
    public ItemAbility itemMainAbility; // 아이템 주 수치 값 
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
    private SkillSaveData skillSaveData = new SkillSaveData();

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


    // 스킬 저장하는 함수 유저 정보를 매개변수로 받는다 
    public void SaveSkillData(string userID)
    {
        if (SkillDataBase.instance == null || saveData == null)
            return;

        // 저장할 주인 ID
        skillSaveData.userID = userID; 

        // 액티브 스킬 저장 
        var activeSkills = SkillDataBase.instance.activeSkillList;

        foreach (var active in activeSkills)
        {
            SkillData skillData = new SkillData();
            skillData.keycode = active.keycode;
            skillData.level = active.MySkillLevel;
            skillData.chainSkill = active.IsChain;

            var  targetSkill = skillSaveData.acitveSkillList.Find(x => x.keycode == skillData.keycode);
            if (targetSkill != null)
            {
                targetSkill.level = skillData.level;
                targetSkill.chainSkill = skillData.chainSkill;
            }
            else
            {
                skillSaveData.acitveSkillList.Add(skillData);
            }
        }

        // 패시브 스킬 저장
        var passiveSkills = SkillDataBase.instance.passiveSkillList;
        foreach (var passive in passiveSkills)
        {
            SkillData skillData = new SkillData();
            skillData.keycode = passive.keycode;
            skillData.level = passive.MySkillLevel;
            skillData.chainSkill = passive.IsChain;

            var targetSkill = skillSaveData.passiveSkillList.Find(x => x.keycode == skillData.keycode);
            if (targetSkill != null)
            {
                targetSkill.level = skillData.level;
                targetSkill.chainSkill = skillData.chainSkill;
            }
            else
            {
                skillSaveData.passiveSkillList.Add(skillData);
            }
        }

        // dictionary 값을 json으로 컨버팅
        string json = JsonConvert.SerializeObject(skillSaveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + INFO_HAVE_SKILL_SAVE_FILENAME, json);

        Debug.Log(json);

    }

    // 저장한 스킬 정보를 게임에 적용한다.
    public void ApplySkillData(string userID)
    {
        if (SkillDataBase.instance == null || skillSaveData == null)
        {
            Debug.Log("해당 클래스가 존재하지 않습니다.");
            return;
        }

        // 액티브 스킬 
        foreach(var active in skillSaveData.acitveSkillList)
        {
            if (active == null)
                continue;

            SkillDataBase.instance.SetActiveSkill(active.keycode, active.level, active.chainSkill);
        }

        // 패시브 스킬
        foreach(var passive in skillSaveData.passiveSkillList)
        {
            if (passive == null)
                continue;

            SkillDataBase.instance.SetPassiveSkill(passive.keycode, passive.level);
        }
        
    }

    // 휠러 정보 저장
    public void SaveWheelers()
    {

        // 자신이 소유하고 있는 캐릭터들 저장 
        List<Character> characters = InfoManager.instance.GetMyPlayerInfoList();
        if (characters.Count > 0)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                WheelerData wheeler = new WheelerData();
                wheeler.wheelerID = characters[i].MyID;
                wheeler.exp = characters[i].MyStat.exp;
                wheeler.maxExp = characters[i].MyStat.maxExp;

                // 장비 타입 값이 1~7까지 잇으므로 순회
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

                // 장착한 스킬 정보 
                for (SkillSlotNumber j = SkillSlotNumber.SLOT1; j <= SkillSlotNumber.MAXSLOT; j++)
                {
                    SkillData skillData = new SkillData();

                    if (characters[i].skills[j] != null)
                    {
                        skillData.keycode = characters[i].skills[j].keycode;
                        skillData.level = characters[i].skills[j].MySkillLevel;
                        skillData.chainSkill = characters[i].skills[j].isChain;
                    }

                    wheeler.skills.Add(skillData);
                }
                // 장착한 체인 스킬 정보
                for (SkillSlotNumber k = SkillSlotNumber.CHAIN1; k <= SkillSlotNumber.MAXCHAINSLOT; k++)
                {
                    SkillData skillData = new SkillData();

                    if (characters[i].chainsSkills[k] != null)
                    {
                        skillData.keycode = characters[i].chainsSkills[k].keycode;
                        skillData.level = characters[i].chainsSkills[k].MySkillLevel;
                        skillData.chainSkill = characters[i].chainsSkills[k].isChain;
                    }

                    wheeler.chainSkills.Add(skillData);
                }

                // 패시브 스킬 정보 
                for (int idx = 0; idx < characters[i].equippedPassiveSkills.Count; idx++)
                {
                    if (characters[i].equippedPassiveSkills[idx] == null)
                    {
                        continue;
                    }

                    SkillData skillData = new SkillData();
                    skillData.keycode = characters[i].equippedPassiveSkills[idx].keycode;
                    skillData.level = characters[i].equippedPassiveSkills[idx].MySkillLevel;

                    wheeler.passvieSkills.Add(skillData);
                }


                if (saveData.wheelerDatas.ContainsKey(wheeler.wheelerID) == true)
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


    // 저장한 값을 게임에 적용한다.
    public void ApplyWheelers()
    {
        foreach (var wheelerPair in saveData.wheelerDatas)
        {
            if (wheelerPair.Value == null) continue;
            var wheeler = wheelerPair.Value;
            CharStat charStat = PlayerDatabase.instance.GetCharStatByWheelerID(wheeler.wheelerID);
            charStat.level = wheeler.level;
            Character tempPlayer = new Character();
            tempPlayer.MyID = wheeler.wheelerID;
            tempPlayer.objectID = wheeler.wheelerID;
            // 스탯 적용
            tempPlayer.MyStat = charStat;
            // 장비 장착
            foreach (var equipItemData in wheeler.equipItems)
            {
                var equipItem = Inventory.instance.GetItemByUniqueID(equipItemData.uniqueID);
                tempPlayer.EquipItem(equipItem as EquipItem);
            }

            // 스킬 장착 
            if (SkillDataBase.instance != null)
            {

                int count = 0;
                foreach (var skill in wheeler.skills)
                {
                    if(skill != null)
                    {
                        var activeSkill = SkillDataBase.instance.GetActiveSkillBySkillKeycode(skill.keycode);
                        tempPlayer.EquipSkill((SkillSlotNumber)count, activeSkill, skill.chainSkill);
                    }
                    count++;
                }

                foreach (var skill in wheeler.passvieSkills)
                {
                    if (skill == null)
                        continue; 
                    var passiveSkill = SkillDataBase.instance.GetActiveSkillBySkillKeycode(skill.keycode);
                    tempPlayer.equippedPassiveSkills.Add(passiveSkill as PassiveSkill);
                }

                count = 0;
                foreach (var skill in wheeler.chainSkills)
                {
                    if(skill != null)
                    {
                        var activeSkill = SkillDataBase.instance.GetActiveSkillBySkillKeycode(skill.keycode);
                        tempPlayer.chainsSkills[(SkillSlotNumber)count] = activeSkill;
                    }
                    count++;
                }
            }

            InfoManager.instance.AddMyPlayerInfo(wheeler.wheelerID, tempPlayer);
        }
    }



    public void SaveInventory()
    {

        for (int i = 0; i < (int)InventoryCategory.MAX; i++)
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

    // 저장한 인벤토리를 게임에 적용
    public void ApplyInvetory()
    {
        InventoryManager.instance.ApplySaveItemData(saveData.inventory);
    }



    // 사용자 정보를 저장한다
    public void SaveUserInfo()
    {
        // 사용 금액 저장 
        saveData.money = InfoManager.coin;

        if (isExistFile == false)
        {
            // 유저 ID 저장  
            saveData.userID = GenerateRandomString(8);
        }

        SaveWheelers();

        SaveInventory();
    }


    public void SaveData()
    {

        // 사운드 관련 
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume* 10) / 10;

        // 탐사 관련 값 
        saveData.isAdventure = StageInfoManager.FLAG_ADVENTURE_MODE;
        saveData.initJoinFlag = StageInfoManager.initJoinPlayGameModeFlag;

        saveData.currentChapter = StageInfoManager.instance.currentChapter;
        saveData.stageDictList = StageInfoManager.instance.GetStageList();


        // 레코드 관련 저장 
        saveData.choiceRecord = RecordManager.CHOICED_COMPLETE_RECORD;
        saveData.recordList.Clear();
        foreach (var record in RecordManager.instance.selectRecordInfos)
        {
            if (record == null) continue; 
            saveData.recordList.Add(record.id);
        }

        // 유저 정보 저장 
        SaveUserInfo();

        // 유저 스킬 정보 저장
        SaveSkillData(saveData.userID);

       // dictionary 값을 json으로 컨버팅
        string json = JsonConvert.SerializeObject(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME, json);

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

    
    // 저장한 정보를 인게임에 적용한다.

    public void ApplyUserInfo()
    {
        InfoManager.coin = saveData.money;
        
        ApplyInvetory();
        
        ApplyWheelers();
    }

    
    

    public void SaveSoundData()
    {
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume * 10) / 10;
    }

    public void LoadSkillData(string userID)
    {
        if(File.Exists(SAVE_DATA_DIRECTROTY + INFO_HAVE_SKILL_SAVE_FILENAME))
        {
            string data = File.ReadAllText(SAVE_DATA_DIRECTROTY + INFO_HAVE_SKILL_SAVE_FILENAME);

            skillSaveData = JsonConvert.DeserializeObject<SkillSaveData>(data);

            ApplySkillData(userID);
        }
        else
        {
            Debug.Log("데이터가 없습니다.");
        }
    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTROTY + SAVE_FILENAME))
        {
            isExistFile = true;
            string data = File.ReadAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME);
            
            saveData = JsonConvert.DeserializeObject<SaveData>(data);
            
            LoadSoundData();

            StageInfoManager.FLAG_ADVENTURE_MODE = saveData.isAdventure;
            StageInfoManager.initJoinPlayGameModeFlag = saveData.initJoinFlag;
            StageInfoManager.instance.currentChapter = saveData.currentChapter;
            StageInfoManager.instance.SetStageList(saveData.stageDictList);

            // 선택한 레코드 위치 값 
            RecordManager.CHOICED_COMPLETE_RECORD = saveData.choiceRecord;
            // 저장한 레코드를 불러온다
            foreach (var id in saveData.recordList)
            {
              RecordManager.instance.SelectRecord(id);
            }

            LoadSkillData(saveData.userID);

            // 유저 정보 적용
            ApplyUserInfo();
        }
        // 저장한 데이터가 없다면 
        else
        {
            isExistFile = false;
            
            Debug.Log("Don't have a save file");

            // todo 
            InfoManager.instance.AddMyPlayerInfo(1);
            InfoManager.instance.AddMyPlayerInfo(101);
            InfoManager.instance.AddMyPlayerInfo(102);
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
