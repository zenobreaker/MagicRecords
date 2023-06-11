using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public int money;
    public float bgmSoundValue;
    public float sfxSoundValue;
    public List<bool> recipeUnlockList = new List<bool>();
    public List<int> weaponItemList = new List<int>();
    public List<int> wItemCountList = new List<int>();
    public List<string> materialItemList = new List<string>();
    public List<int> mItemCountList = new List<int>();
    public List<bool> stroyViewList = new List<bool>();
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
        //if(theInven == null)
        //    theInven = FindObjectOfType<Inventory>();
        //if (theRecipe == null)
        //    theRecipe = FindObjectOfType<RecipePage>();
        //if (theSC == null)
        //    theSC = FindObjectOfType<SoundController>();

        //Item[] wItems = theInven.SaveWeaponData();
        //Item[] mItems = theInven.SaveMaterialData();
        //bool[] viewArr = theIC.GetViewList().ToArray();
        //bool[] beingArr = theQuest.GetQuestBeingList().ToArray();
        //bool[] clearArr = theQuest.GetQuestClearList().ToArray();

        //saveData.money = GameManager.instance.money;
        
        // 사운드 저장 
        //saveData.sfxSoundValue = Mathf.Floor(theSC.sfxSlider.value * 10) / 10;
        //saveData.bgmSoundValue = Mathf.Floor(theSC.bgmSlider.value * 10) / 10;

        // 아이템 저장 
        //for (int i = 0; i < wItems.Length; i++)
        //{
        //    if (saveData.weaponItemList.Contains(wItems[i].itemID))
        //    {
        //        int idx = saveData.weaponItemList.IndexOf(wItems[i].itemID);
        //        saveData.wItemCountList[idx] = wItems[idx].itemCount;
        //    }
        //    else
        //    {
        //        saveData.weaponItemList.Add(wItems[i].itemID);
        //        saveData.wItemCountList.Add(wItems[i].itemCount);
        //    }
        //}

        //for (int j = 0; j < mItems.Length; j++)
        //{
        //    if (saveData.materialItemList.Contains(mItems[j].itemID))
        //    {
        //        int idx = saveData.materialItemList.IndexOf(mItems[j].itemID);
        //        saveData.mItemCountList[idx] = mItems[idx].itemCount;
        //    }
        //    else
        //    {
        //        saveData.materialItemList.Add(mItems[j].itemID);
        //        saveData.mItemCountList.Add(mItems[j].itemCount);
        //    }
        //}

   

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTROTY + SAVE_FILENAME, json);

        //Debug.Log("저장 완료");
        //Debug.Log(json);
    }

    // 캐릭터 저장
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
            // todo 플레이어 클래스 내부를 또 나눠서 저장시켜놓기 
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

        // 무기 
        Item[] wItems = theInven.GetSaveWeaponData();
        // 방어구 
        //Item[] aItems = theInven.SaveMaterialData();

        for (int i = 0; i < wItems.Length; i++)
        {
            if (saveData.weaponItemList.Contains(wItems[i].itemUID))
            {
                int idx = saveData.weaponItemList.IndexOf(wItems[i].itemUID);
                saveData.wItemCountList[idx] = wItems[idx].itemEach;
            }
            else
            {
                saveData.weaponItemList.Add(wItems[i].itemUID);
                saveData.wItemCountList.Add(wItems[i].itemEach);
            }
        }


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

        //    // Debug.Log("로드 완료");
        //    theIC.StartFirstStroy();
        //}
        //else
        //{
        //    GameManager.money = 0;
        //    UIManager.instance.SetMoney(GameManager.money);
        //    theIC.StartFirstStroy();
        //    //  Debug.Log("저장된 파일이 없습니다.");
        //}
        go_BackGround.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        //if (!SceneManager.GetActiveScene().name.Equals("Title"))
        //    SaveData();
    }
}
