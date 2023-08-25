using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using UnityEditor;

[System.Serializable]
public class SaveData
{
    public float bgmSoundValue;
    public float sfxSoundValue;

    public UserData userdata;

    public List<int> weaponItemList = new List<int>();
    public List<int> wItemCountList = new List<int>();
    public List<int> mItemCountList = new List<int>();
}


// ���� ���� ����
[System.Serializable]
public class UserData
{
    public string userID;
    public int money;   // ������ ���� �ִ� �Ӵ�
    // ������ ������ �ִ� ĳ���͵� 
    public List<WheelerData> wheelerDatas = new List<WheelerData>();
    // �κ��丮
    public List<ItemData> inventory = new List<ItemData>();

}

// �÷��̾�� ĳ���� ���� 
[System.Serializable]
public class WheelerData
{
    public int saveDataID;  // �� �����͸� ���� data ������ ID
    public int wheelerID;   // ��� ĳ���� ID
    //ĳ������ ���ȵ� 
    public int level;
    public int attack;
    public int defense;
    public float attackSpeed;
    public int hp;
    public int mp;
    public int hpRegen;
    public int mpRegen;
    public int speed;
    public float critRate;
    public float critDamage;

    public int exp;
    public int maxExp;

    // ����Ʈ�� ������ ������ ��� ��ų � ���� �ܺ� �������� ó���ǹǷ� �������
    // ������ ��� id�� �����ϰ� �κ��丮���� �ش� id�� �������� �����ͼ� ������Ų��
    public List<EquipItemData> equipItems = new List<EquipItemData>();

    // ������ ��� 
    public int droneID; 

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
    private string SAVE_FILENAME = "/SaveFile.json";
    private string INFO_SAVE_FILENAME = "/InfoSaveFile.json";
    private string INFO_HAVE_SKILL_SAVE_FILENAME = "/HaveSkillSaveFile.json";
    private string INFO_ITEM_SAVE_FILENAME = "/HaveITEMSaveFile.json";

    [SerializeField] private Inventory theInven;
    [SerializeField] InteractionController theIC = null;
    [SerializeField] SoundManager soundManager = null;
    //[SerializeField] QuestManager theQuest = null;

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

     
        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME, json);

        //Debug.Log("���� �Ϸ�");
        //Debug.Log(json);
    }


    // �÷��̾� ���� ���� ����
    public void SaveUserInfo()
    {
        // �÷����� ���� ������ ���� 
        var userdata = new UserData();
        userdata.money = InfoManager.instance.money;

        SaveWheelers(userdata);

        SaveInventory(userdata); 
    }


    public void SaveWheelers(UserData userData)
    {
        if (userData == null)
            return;

        // ������ �ִ� ĳ���͵� ����
        List<Character> characters = InfoManager.instance.GetMyPlayerInfoList();
        if (characters.Count > 0)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                WheelerData wheeler = new WheelerData();
                wheeler.wheelerID = characters[i].MyID;
                wheeler.attack = characters[i].MyStat.attack;
                wheeler.defense = characters[i].MyStat.defense;
                wheeler.attackSpeed = characters[i].MyStat.attackSpeed;
                wheeler.speed = characters[i].MyStat.speed;
                wheeler.hp = characters[i].MyStat.hp;
                wheeler.mp = characters[i].MyStat.mp;
                wheeler.hpRegen = characters[i].MyStat.hpRegen;
                wheeler.mpRegen = characters[i].MyStat.mpRegen;
                wheeler.exp = characters[i].MyStat.exp;
                wheeler.maxExp = characters[i].MyStat.maxExp;
                wheeler.critRate = characters[i].MyStat.critRate;
                wheeler.critDamage = characters[i].MyStat.critDmg;


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

                userData.wheelerDatas.Add(wheeler);
            }
        }
    }

    public void SaveInventory(UserData userData)
    {
        if (userData == null)
            return; 

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
                
                userData.inventory.Add(itemData);
            }
        }

    }

    // ĳ���� ����
    void SaveCharacterData()
    {
        //BinaryFormatter bf = new BinaryFormatter();
        //var path = Path.Combine(Application.dataPath + "/Data/", "database.json");
        //Path.Combine(Application.persistentDataPath, "database.json");
        //FileStream file = File.Create(path);

        var saveInfoList = InfoManager.instance.GetMyPlayerInfoList();
        foreach (var info in saveInfoList)
        {
            //bf.Serialize(file, info);
            // todo �÷��̾� Ŭ���� ���θ� �� ������ ������ѳ��� 
            string json = JsonUtility.ToJson(info);
            File.WriteAllText(SAVE_DATA_DIRECTROTY + INFO_SAVE_FILENAME, json);
        }

        //file.Close();
    }


    

    public void SaveSoundData()
    {
        
        saveData.sfxSoundValue = Mathf.Floor(SoundManager.instance.sfxVolume * 10) / 10;
        saveData.bgmSoundValue = Mathf.Floor(SoundManager.instance.bgmVolume * 10) / 10;
    }

    public void SaveItems()
    {

        // ���� 
        //Item[] wItems = theInven.GetSaveWeaponData();
        // �� 
        //Item[] aItems = theInven.SaveMaterialData();


        //saveData.money = GameManager.money;
    }

    public void LoadData()
    {
    //    if (SceneManager.GetActiveScene().name.Equals("Title"))
    //        LoadSoundData();
    //    else
    //        StartCoroutine(LoadingData());
    //        StartCoroutine(LoadingData());
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

    IEnumerator LoadingData()
    {
        go_BackGround.SetActive(true);

        //if (File.Exists(SAVE_DATA_DIRECTROTY + SAVE_FILENAME))
        //{
        //    string loadJson = File.ReadAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME);
        //    saveData = JsonUtility.FromJson<SaveData>(loadJson);

        //    //if (theInven == null)
        //    //    theInven = FindObjectOfType<Inventory>();
        //    //if (theRecipe == null)
        //    //    theRecipe = FindObjectOfType<RecipePage>();
        //    //if (theSC == null)
        //    //    theSC = FindObjectOfType<SoundController>();

        //    for (int i = 0; i < saveData.weaponItemList.Count; i++)
        //    {
        //        theInven.LoadWeaponData(saveData.weaponItemList[i], saveData.wItemCountList[i]);
        //        yield return null;
        //    }

        //    for (int x = 0; x < saveData.materialItemList.Count; x++)
        //    {
        //        theInven.LoadMaterialData(saveData.materialItemList[x], saveData.mItemCountList[x]);
        //        yield return null;
        //    }

        //    theRecipe.LoadRecipeData(saveData.recipeUnlockList);

        //    GameManager.money = saveData.money;
        //    UIManager.instance.SetMoney(saveData.money);

        //    theSC.SetSfxVolume(saveData.sfxSoundValue);
        //    theSC.SetBGMVolume(saveData.bgmSoundValue);
        //    theIC.SetViewList(saveData.stroyViewList);
        //    theQuest.SetQuestDic(saveData.questBeingList, saveData.questClearList);

            yield return null;

        //    // Debug.Log("�ε� �Ϸ�");
        //    theIC.StartFirstStroy();
        //}
        //else
        //{
        //    GameManager.money = 0;
        //    UIManager.instance.SetMoney(GameManager.money);
        //    theIC.StartFirstStroy();
        //    //  Debug.Log("����� ������ �����ϴ�.");
        //}
        go_BackGround.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        //if (!SceneManager.GetActiveScene().name.Equals("Title"))
        //    SaveData();
    }
}
