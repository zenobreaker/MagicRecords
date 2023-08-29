using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





// 게임내 사용될 메모리 클래스
[System.Serializable]
public class RecordInfo
{
    public int id;          // 식별 id
    public string name;     // 레코드 이름
    public string description; // 레코드설명 
    public int grade;       // 레코드 등급
    public int specialOptionID;
    public string spritePath;   // 레코드 스프라이트 경로
    public SpecialOption specialOption; // 레코드 효과 
    public RecordInfo(int id, string name, string description, int grade, int specialOptionID)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.grade = grade;
        this.specialOptionID = specialOptionID;
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
    public int specialOptionID;
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
    public static bool CHOICED_COMPLETE_RECORD = false; // 레코드 선택완료에 대한 값
    [Header("메모리 정보 JSON 데이터")]
    public TextAsset memoryInfoJsonData;

    private RecordInfoJsonAllData memoyInfoJsonAlldata;


    public Dictionary<int, RecordInfo> recordInfoDictionary = new Dictionary<int, RecordInfo>();

    // 게임 중에 선택한 레코드 리스트
    public List<RecordInfo> selectRecordInfos = new List<RecordInfo>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

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
                memInfoJson.description, memInfoJson.grade, memInfoJson.specialOptionID);

            // 옵션매니저에서 가져올려는 옵션이 있는지 검사
            if (OptionManager.instance != null)
            {
                // 해당 info 클래스에 옵션 클래스를 넣어둔다.
                memoryInfo.specialOption = OptionManager.instance.GetSpecialOption(memInfoJson.specialOptionID);
            }

            // 리스트에 추가하기 
            recordInfoDictionary.Add(memInfoJson.id, memoryInfo);   
        }
    }

    // 특정 ID 값을 받으면 해당 메모리를 반환하는 함수
    public RecordInfo GetRecordInfoByID(int id)
    {
        if (recordInfoDictionary.ContainsKey(id) == false)
            return null; 

        return recordInfoDictionary[id];
    }

    // 메모리를 랜덤으로 받는 매개변수만큼 반환해주는 함수 
    public List<RecordInfo> GetRandomRewardMemory(int count)
    {
        List<RecordInfo> rewardList = new List<RecordInfo>();


        // 1. 생성할 메모리 id를 딕셔너리에서 중복 없이 가져온다. 
        List<int> keys= new List<int>(recordInfoDictionary.Keys);
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

            if(prevIndx == idx || rewardList.Contains(recordInfoDictionary[keys[idx]]) == true)
            {
                continue; 
            }

            if(recordInfoDictionary.ContainsKey(idx) == true)
            {
                // 2. 만든 리스트를 토대로 메모리를 가져와 전달한다.
                rewardList.Add(recordInfoDictionary[idx]);
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

            int id = rewardList[i].specialOptionID;

            rewardList[i].specialOption = OptionManager.instance.GetSpecialOption(id);
        }

        return rewardList;
    }

    public SpecialOption GetSpecialOptionToRecordInfo(int id)
    {
        var record = recordInfoDictionary[id];
        if (record == null) return null;


        record.specialOption =  OptionManager.instance.GetSpecialOption(record.specialOptionID);

        return record.specialOption;
    }


    // 아래 코드는 레코드가 개별로 적용할 때 사용할 코드였다. 지금은 모든 플레이어에게 적용할 예정이기에 
    // 효용성이 떨어지므로 주석처리한다. 
    // 필드에 나와있는 플레이어들에게 레코드를 적용하는 함수
    //public void ApplyRecordAbilityToAllPlayers(List<Character> players)
    //{
    //    for (int i = selectRecordInfos.Count - 1; i >= 0; i--)
    //    {
    //        RecordInfo record = selectRecordInfos[i];
    //        record.specialOption ??= GetSpecialOptionToRecordInfo(record.optionID);
    //        foreach (Character player in players)
    //        {
    //            player.ApplyRecordAbility(record);
    //            record.specialOption.SetCoolTime();
    //        }
    //    }
    //}

    // 레코드를 선택하는 함수
    public void SelectRecord(int recordID)
    {
        if (recordID == 0) return;

        if (recordInfoDictionary.ContainsKey(recordID) == false) return;

        SelectRecord(recordInfoDictionary[recordID]);
    }

    public void SelectRecord(RecordInfo selectedRecord)
    {
        selectRecordInfos.Add(selectedRecord);
    }

    // 플레이어들에게 레코드 적용하기
    public void ApplyRecordToPlayers(List<CharacterController> players)
    {
        for (int i = selectRecordInfos.Count - 1; i >= 0; i--)
        {
            RecordInfo record = selectRecordInfos[i];
            // ?? specialOption 가 null 이라면 오른쪽에 연산을 통하라는 뜻 
            record.specialOption ??= GetSpecialOptionToRecordInfo(record.specialOptionID);

            foreach (var player in players)
            {
                if (player.MyPlayer == null) continue;
                
                // 아래 코루틴 실행 
                StartCoroutine(ManageRecordTimer(player.MyPlayer, record));
            }
        }
    }



    public IEnumerator ManageRecordTimer(Character player, RecordInfo record)
    {
        if (record.specialOption == null || player == null)
            yield return null;

        player.ApplyRecordAbility(record);

        yield return new WaitForSeconds(record.specialOption.duration);

        player.RemoveRecordAbility(record);

        yield return new WaitForSeconds(record.specialOption.coolTime);

        // todo 쿨타임이 끝난 후 처리 로직 추가 자리 
    }

    // 업데이트 문에서 동작하는 함수 
    // 적용한 레코드들을 순회하며 쿨타임을 잰다 
    public void UpdateRecordTiemers()
    {
        for (int i = selectRecordInfos.Count - 1; i >= 0; i--)
        {
            RecordInfo record = selectRecordInfos[i];
            // ?? specialOption 가 null 이라면 오른쪽에 연산을 통하라는 뜻 
            record.specialOption ??= GetSpecialOptionToRecordInfo(record.specialOptionID);

            if (record.specialOption.coolTime <= 0)
            {

            }

        }
    }
    // 선택한 레코드들을 다 초기화한다. 
    public void ClearRecords()
    {
        selectRecordInfos.Clear();  
    }
}
