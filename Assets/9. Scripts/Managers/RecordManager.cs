using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





// 게임내 사용될 메모리 클래스
[System.Serializable]
public class RecordInfo
{
    public int id;          // 식별 id
    public string name;     // 메모리 이름
    public string description; // 메모리 설명 
    public int grade;       // 메모리 등급
    public int optionID;
    public string spritePath;   // 메모리 스프라이트 경로
    public SpecialOption specialOption; // 메모리 효과 

    public RecordInfo(int id, string name, string description, int grade, int optionID)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.grade = grade;
        this.optionID = optionID;
    }

    public RecordInfo(int id, string name, string description, int grade,
        SpecialOption specialEffect)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.grade = grade;
        this.specialOption = specialEffect;
    }
}

// Memory 정보를 가진 Json 클래스
[System.Serializable]
public class RecordInfoJson
{
    public int id;
    public string namekeycode;
    public string description;
    public int grade;
    public int optionID;
}

// RecordInfoJson 클래스를 리스트로 담고 있는 클래스 
[System.Serializable]
public class RecordInfoJsonAllData
{
    public RecordInfoJson[] recordInfoJson;
}

// 게임 내 출현하는 메모리 관리하는 매니저

public class RecordManager : MonoBehaviour
{

    public static RecordManager instance; 

    [Header("메모리 정보 JSON 데이터")]
    public TextAsset memoryInfoJsonData;

    private RecordInfoJsonAllData memoyInfoJsonAlldata;


    public Dictionary<int, RecordInfo> memoryInfoDictionary = new Dictionary<int, RecordInfo>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

        InitializeRecordInfo();
    }


    // RecordInfo 초기화 
    public void InitializeRecordInfo()
    {
        memoyInfoJsonAlldata = JsonUtility.FromJson<RecordInfoJsonAllData>(memoryInfoJsonData.text);
        if (memoyInfoJsonAlldata == null ||
            memoyInfoJsonAlldata.recordInfoJson == null)
            return; 

        foreach(RecordInfoJson memInfoJson in memoyInfoJsonAlldata.recordInfoJson)
        {
            if(memInfoJson == null) continue;

            RecordInfo memoryInfo = new RecordInfo(memInfoJson.id, memInfoJson.namekeycode, 
                memInfoJson.description, memInfoJson.grade, memInfoJson.optionID);

            // 옵션매니저에서 가져올려는 옵션이 있는지 검사
            if(OptionManager.instance != null)
            {
                if(OptionManager.instance.specialOptionsDictionary.ContainsKey(memInfoJson.namekeycode) == true)
                {
                    // 해당 info 클래스에 옵션 클래스를 넣어둔다.
                    memoryInfo.specialOption = OptionManager.instance.specialOptionsDictionary[memInfoJson.namekeycode];
                }
            }

            // 리스트에 추가하기 
            memoryInfoDictionary.Add(memInfoJson.id, memoryInfo);   
        }
    }

    // 특정 ID 값을 받으면 해당 메모리를 반환하는 함수
    public RecordInfo GetRecordInfoByID(int id)
    {
        if (memoryInfoDictionary.ContainsKey(id) == false)
            return null; 

        return memoryInfoDictionary[id];
    }

    // 메모리를 랜덤으로 받는 매개변수만큼 반환해주는 함수 
    public List<RecordInfo> GetRandomRewardMemory(int count)
    {
        List<RecordInfo> rewardList = new List<RecordInfo>();


        // 1. 생성할 메모리 id를 딕셔너리에서 중복 없이 가져온다. 
        List<int> keys= new List<int>(memoryInfoDictionary.Keys);
        if (keys.Count <= 0)
        {
            Debug.Log("키값이 없습니다 게임 진행 불가");
            return null;
        }
        int min = keys.Min();
        int maxCount = keys.Count;
      

        // 중복없이 갯수만큼 리스트에 추가하기 
        int prevIndx = 0; 
        while (true)
        {

            int idx = Random.Range(min, maxCount);

            if(prevIndx == idx || rewardList.Contains(memoryInfoDictionary[keys[idx]]) == true)
            {
                continue; 
            }

            if(memoryInfoDictionary.ContainsKey(idx) == true)
            {
                // 2. 만든 리스트를 토대로 메모리를 가져와 전달한다.
                rewardList.Add(memoryInfoDictionary[idx]);
            }

            if(rewardList.Count >= count)
                break; 
        }


        // 메모리를 검사해서 옵션 데이터가 있는지 검사 없으면 만들어서 전달
        for (int i = 0; i < count; i++)
        {
            if (rewardList[i].specialOption != null) continue;

            // id 정보는 있을터이니 그것을 통해 optionmanager에게서 가져온다. 
            if(OptionManager.instance == null)
            {
                Debug.Log("옵션매니저가 없습니다.");
                break; 
            }

            int id = rewardList[i].optionID;

            rewardList[i].specialOption = OptionManager.instance.GetSpecialOption(id);
        }

        return rewardList;
    }

}
