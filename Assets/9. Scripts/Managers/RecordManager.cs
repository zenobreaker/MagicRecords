
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;





// 레코드 정보 
[System.Serializable]
public class RecordInfo
{
    public int id;          //  id
    public string name;     // 레코드 이름
    public string description; // 레코드 설명
    public int grade;       // 레코드 등급
    public int specialOptionID;
    public string spritePath;   // 레코드 이미지 경로
    public SpecialOption specialOption; // 레코드 옵션
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

// 레코드 정보 json 클래스 
[System.Serializable]
public class RecordInfoJson
{
    public int id;
    public string namekeycode;
    public string description;
    public int grade;
    public int specialOptionID;
}

// RecordInfoJson 를 여럿 담고 있는 클래스
[System.Serializable]
public class RecordInfoJsonAllData
{
    public RecordInfoJson[] recordInfoJson;
}

// 레코드를 관리하는 매니저 
public class RecordManager : MonoBehaviour
{

    public static RecordManager instance;
    public static bool CHOICED_COMPLETE_RECORD = false; // 레코드를 선택을 완료했는지에 대한 체크값
    [Header("�޸� ���� JSON ������")]
    public TextAsset memoryInfoJsonData;

    private RecordInfoJsonAllData memoyInfoJsonAlldata;

    public Dictionary<int, RecordInfo> recordInfoDictionary = new Dictionary<int, RecordInfo>();

    // 선택한 레코드들 
    public List<RecordInfo> selectRecordInfos = new List<RecordInfo>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        InitializeRecordInfo();
    }


    // RecordInfo �ʱ�ȭ 
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

            // �ɼǸŴ������� �����÷��� �ɼ��� �ִ��� �˻�
            if (OptionManager.instance != null)
            {
                // �ش� info Ŭ������ �ɼ� Ŭ������ �־�д�.
                memoryInfo.specialOption = OptionManager.instance.GetSpecialOption(memInfoJson.specialOptionID);
            }

            // ����Ʈ�� �߰��ϱ� 
            recordInfoDictionary.Add(memInfoJson.id, memoryInfo);   
        }
    }

    // Ư�� ID ���� ������ �ش� �޸𸮸� ��ȯ�ϴ� �Լ�
    public RecordInfo GetRecordInfoByID(int id)
    {
        if (recordInfoDictionary.ContainsKey(id) == false)
            return null; 

        return recordInfoDictionary[id];
    }

    public List<int> GetRanddomRecordID(int count)
    {
        List<int> resultList = new List<int>();
        
        // 1. 키값으로 리스트를 만든다
        List<int> keys = new List<int>(recordInfoDictionary.Keys);
        if (keys.Count <= 0)
        {
            Debug.Log("등록된 레코드 정보가 없습니다.");
            return null;
        }
        int min = keys.Min();
        int maxCount = keys.Count;
        
        int prevIndx = 0;
        while (true)
        {

            int idx = Random.Range(min, maxCount);

            if (prevIndx == idx || selectRecordInfos.Contains(recordInfoDictionary[keys[idx]]) == true)
            {
                continue;
            }
            if (recordInfoDictionary.ContainsKey(idx) == true)
            {
                // 2. 결과 리스트에 추가
                resultList.Add(recordInfoDictionary[idx].id);
            }

            if (resultList.Count >= count)
                break;
        }


        return resultList;
    }

    // 일정 개수 값을 받으면 랜덤으로 메모리를 반환한다.
    public List<RecordInfo> GetRandomRewardMemory(int count)
    {
        List<RecordInfo> rewardList = new List<RecordInfo>();

        List<int> keys = new List<int>(recordInfoDictionary.Keys);
        if (keys.Count <= 0)
        {
            return null;
        }
        int min = keys.Min();
        int maxCount = keys.Count;
      

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
                rewardList.Add(recordInfoDictionary[idx]);
            }

            if(rewardList.Count >= count)
                break; 
        }


        for (int i = 0; i < count; i++)
        {
            if (rewardList[i].specialOption != null) continue;

            if(OptionManager.instance == null)
            {
                break; 
            }

            int id = rewardList[i].specialOptionID;

            rewardList[i].specialOption = OptionManager.instance.GetSpecialOption(id);
        }

        return rewardList;
    }

    public List<int> GetRandomRecordByRandRange(int start, int end)
    {
        List<int> idList = new List<int>();

        if(end - start == 0)
        {
            return null; 
        }


        int count = Random.Range(start, end + 1);

        List<int> keys = new List<int>(recordInfoDictionary.Keys);
        if (keys.Count <= 0)
        {
            return null;
        }

        int min = keys.Min();
        int maxCount = keys.Count;

        List<RecordInfo> records = new List<RecordInfo>(recordInfoDictionary.Values);

        int prevIndex = 0;
        while (true)
        {
            int id = Random.Range(min, maxCount);
            var record = recordInfoDictionary[id];
            if (prevIndex == id  || idList.Contains(id) == true ||
                selectRecordInfos.Contains(record) == true)
            {
                continue; 
            }

            idList.Add(id);
            records.Remove(record); 
            if (idList.Count >= count || records.Count <= 0)
            {
                break; 
            }
        }


        return idList; 
    }

    public SpecialOption GetSpecialOptionToRecordInfo(int id)
    {
        var record = recordInfoDictionary[id];
        if (record == null) return null;

        record.specialOption =  OptionManager.instance.GetSpecialOption(record.specialOptionID);

        return record.specialOption;
    }

    public void GetStageEventRewardRecord(StageAppearInfo appearInfo)
    {
        if ( appearInfo == null) return;

        appearInfo.appearIDList.Clear();

        List<int> recordIDList = GetRanddomRecordID(3);
        if (recordIDList == null || recordIDList.Count <= 0)
        {
            return;
        }

        foreach(var record in recordIDList)
        {
            appearInfo.appearIDList.Add(record);
        }
    }

    // �Ʒ� �ڵ�� ���ڵ尡 ������ ������ �� ����� �ڵ忴��. ������ ��� �÷��̾�� ������ �����̱⿡ 
    // ȿ�뼺�� �������Ƿ� �ּ�ó���Ѵ�. 
    // �ʵ忡 �����ִ� �÷��̾�鿡�� ���ڵ带 �����ϴ� �Լ�
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

    // ���ڵ带 �����ϴ� �Լ�
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

    // 플레이어들에게 레코드 효과 적용하기
    public void ApplyRecordToPlayers(List<WheelerController> players)
    {
        for (int i = selectRecordInfos.Count - 1; i >= 0; i--)
        {
            RecordInfo record = selectRecordInfos[i];
            // ?? specialOption null 
            record.specialOption ??= GetSpecialOptionToRecordInfo(record.specialOptionID);

            foreach (var player in players)
            {
                if (player == null)
                    continue;
                
                // 레코드 타이머 코루틴 시작 
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

        // todo 타이머가 끝나면 일부 레코드 관련 해서 다른 효과 발휘 등에 내용 추가 
    }

    public void UpdateRecordTiemers()
    {
        for (int i = selectRecordInfos.Count - 1; i >= 0; i--)
        {
            RecordInfo record = selectRecordInfos[i];
            // ?? specialOption �� null
            record.specialOption ??= GetSpecialOptionToRecordInfo(record.specialOptionID);

            if (record.specialOption.coolTime <= 0)
            {

            }

        }
    }

    public void ClearRecords()
    {
        selectRecordInfos.Clear();  
    }
}
