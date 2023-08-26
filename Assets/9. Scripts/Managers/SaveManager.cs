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
    // ������ ������ �ִ� ĳ���͵� 
    public List<WheelerData> wheelerDatas = new List<WheelerData>();
    // �κ��丮
    public List<ItemData> inventory = new List<ItemData>();
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

    // ������ ��� 
    public int droneID = 0; 

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
    public int enhanceCount;    // ��ȭ��ġ 
    public ItemAbility itemMainAbility; // ������ �ɷ� ��ġ 
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

        // ���� ���� 
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume* 10) / 10;


        // ���� ���� ���� 
        SaveUserInfo();

       
        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME, json);

        //Debug.Log("���� �Ϸ�");
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

        // ���ӸӴ� 
        InfoManager.instance.money = saveData.money;
        
        ApplyWheelers();

        ApplyInvetory();
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

                    equipItemData.itemID = characters[i].equipItems[(EquipType)x].itemUID;
                    equipItemData.equipItemID = characters[i].equipItems[(EquipType)x].itemUID;
                    equipItemData.equipType = (EquipType)x;
                    equipItemData.enhanceCount = characters[i].equipItems[(EquipType)x].itemEnchantRank;
                    equipItemData.itemMainAbility = characters[i].equipItems[(EquipType)x].itemMainAbility;
                    equipItemData.itemAbilities = characters[i].equipItems[(EquipType)x].itemAbilities;

                    wheeler.equipItems.Add(equipItemData);
                }


                wheeler.droneID = characters[i].drone.itemUID;

                // ��ų ���� 
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
    

    // ���ӿ� ������ ���� ���� �ٷ� 
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

    // ���ӿ� ������ ���� ���� �κ��丮
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
        // ������ saveCount ���� �����ϴ� ������ ���ٸ� ���Ӱ� �����ϴ� �� 
        // ���� �ִ� ��� 
        if (File.Exists(SAVE_DATA_DIRECTROTY + SAVE_FILENAME))
        {
            isExistFile = true;
            string data = File.ReadAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME);
            // ���Ͽ��� ���̺� ������ ������ �����Ѵ�. 
            saveData = JsonUtility.FromJson<SaveData>(data);

            // ���� ���� ����
            
            // ������ ���� ĳ����(wheeler) ����

            // �κ��丮 ���� ���� 

        }
        // ���� ���� ��� 
        else
        {
            isExistFile = false;
            // ������ ���� ���� 
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
