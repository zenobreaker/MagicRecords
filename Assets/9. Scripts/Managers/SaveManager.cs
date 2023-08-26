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
    public int money = 0;   // 유저가 갖고 있는 머니
    // 유저가 가지고 있는 캐릭터들 
    public List<WheelerData> wheelerDatas = new List<WheelerData>();
    // 인벤토리
    public List<ItemData> inventory = new List<ItemData>();
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

    // 장착한 드론 
    public int droneID = 0; 

    // 장착한 스킬 
    public List<SkillData> skills = new List<SkillData>();
    public List<SkillData> chainSkills = new List<SkillData>();

    // todo 유물? 

}

// 스킬 정보 
[System.Serializable]
public class SkillData
{
    public string name;
    public int level;
    public bool chainSkill;
}

// 아이템 정보
[System.Serializable]
public class ItemData
{
    public int itemID;
    public int count;

    public ItemData()
    {
        itemID = 0;
        count = 0; 
    }
}

[System.Serializable]
public class EquipItemData : ItemData
{
    public int equipItemID;
    public EquipType equipType;
    public int enhanceCount;    // 강화수치 
    public ItemAbility itemMainAbility; // 아이템 능력 수치 
    public ItemAbility[] itemAbilities;

    public EquipItemData()
    {
        equipItemID = 0;
        enhanceCount = 0; 
        itemMainAbility.abilityType = AbilityType.NONE;
        itemMainAbility.power = 0;
        itemMainAbility.isPercent = false;
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

        // 사운드 저장 
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume* 10) / 10;


        // 유저 정보 저장 
        SaveUserInfo();

       
        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME, json);

        //Debug.Log("저장 완료");
        //Debug.Log(json);
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
        saveData.money = InfoManager.instance.money;

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

        // 게임머니 
        InfoManager.instance.money = saveData.money;
        
        ApplyWheelers();

        ApplyInvetory();
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

                    equipItemData.itemID = characters[i].equipItems[(EquipType)x].itemUID;
                    equipItemData.equipItemID = characters[i].equipItems[(EquipType)x].itemUID;
                    equipItemData.equipType = (EquipType)x;
                    equipItemData.enhanceCount = characters[i].equipItems[(EquipType)x].itemEnchantRank;
                    equipItemData.itemMainAbility = characters[i].equipItems[(EquipType)x].itemMainAbility;
                    equipItemData.itemAbilities = characters[i].equipItems[(EquipType)x].itemAbilities;

                    wheeler.equipItems.Add(equipItemData);
                }


                wheeler.droneID = characters[i].drone.itemUID;

                // 스킬 정보 
                for (int j = 0; j < characters[i].MySkills.Length; j++)
                {
                    SkillData skillData = new SkillData();

                    skillData.name = characters[i].MySkills[j].MyName;
                    skillData.level = characters[i].MySkills[j].MySkillLevel;
                    skillData.chainSkill = characters[i].MySkills[j].isChain;

                    wheeler.skills.Add(skillData);
                }

                for (int k = 0; k < characters[i].MyChains.Length; k++)
                {
                    SkillData skillData = new SkillData();

                    skillData.name = characters[i].MyChains[k].MyName;
                    skillData.level = characters[i].MyChains[k].MySkillLevel;
                    skillData.chainSkill = characters[i].MyChains[k].isChain;

                    wheeler.chainSkills.Add(skillData);
                }

                saveData.wheelerDatas.Add(wheeler);
            }
        }
    }
    

    // 게임에 저장한 정보 세팅 휠러 
    public void  ApplyWheelers()
    {
        foreach(var wheeler in saveData.wheelerDatas)
        {
            if (wheeler == null) continue; 

            

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
                itemData.itemID = itemList[i].itemUID;
                itemData.count = itemList[i].itemEach;
                
                saveData.inventory.Add(itemData);
            }
        }

    }

    // 게임에 저장한 정보 세팅 인벤토리
    public void ApplyInvetory()
    {

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
            saveData = JsonUtility.FromJson<SaveData>(data);

            // 유저 정보 세팅
            
            // 유저가 가진 캐릭터(wheeler) 세팅

            // 인벤토리 정보 세팅 

        }
        // 값이 없는 경우 
        else
        {
            isExistFile = false;
            // 없으면 새로 시작 
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
        //    SaveData();
    }
}
