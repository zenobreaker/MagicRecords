using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





// ���ӳ� ���� �޸� Ŭ����
[System.Serializable]
public class RecordInfo
{
    public int id;          // �ĺ� id
    public string name;     // ���ڵ� �̸�
    public string description; // ���ڵ弳�� 
    public int grade;       // ���ڵ� ���
    public int specialOptionID;
    public string spritePath;   // ���ڵ� ��������Ʈ ���
    public SpecialOption specialOption; // ���ڵ� ȿ�� 
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

// Memory ������ ���� Json Ŭ����
[System.Serializable]
public class RecordInfoJson
{
    public int id;
    public string namekeycode;
    public string description;
    public int grade;
    public int specialOptionID;
}

// RecordInfoJson Ŭ������ ����Ʈ�� ��� �ִ� Ŭ���� 
[System.Serializable]
public class RecordInfoJsonAllData
{
    public RecordInfoJson[] recordInfoJson;
}

// ���� �� �����ϴ� �޸� �����ϴ� �Ŵ���

public class RecordManager : MonoBehaviour
{

    public static RecordManager instance;
    public static bool CHOICED_COMPLETE_RECORD = false; // ���ڵ� ���ÿϷῡ ���� ��
    [Header("�޸� ���� JSON ������")]
    public TextAsset memoryInfoJsonData;

    private RecordInfoJsonAllData memoyInfoJsonAlldata;


    public Dictionary<int, RecordInfo> recordInfoDictionary = new Dictionary<int, RecordInfo>();

    // ���� �߿� ������ ���ڵ� ����Ʈ
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

    // �޸𸮸� �������� �޴� �Ű�������ŭ ��ȯ���ִ� �Լ� 
    public List<RecordInfo> GetRandomRewardMemory(int count)
    {
        List<RecordInfo> rewardList = new List<RecordInfo>();


        // 1. ������ �޸� id�� ��ųʸ����� �ߺ� ���� �����´�. 
        List<int> keys= new List<int>(recordInfoDictionary.Keys);
        if (keys.Count <= 0)
        {
            Debug.Log("Ű���� �����ϴ� ���� ���� �Ұ�");
            return null;
        }
        int min = keys.Min();
        int maxCount = keys.Count;
      

        // �ߺ����� ������ŭ ����Ʈ�� �߰��ϱ� 
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
                // 2. ���� ����Ʈ�� ���� �޸𸮸� ������ �����Ѵ�.
                rewardList.Add(recordInfoDictionary[idx]);
            }

            if(rewardList.Count >= count)
                break; 
        }


        // �޸𸮸� �˻��ؼ� �ɼ� �����Ͱ� �ִ��� �˻� ������ ���� ����
        for (int i = 0; i < count; i++)
        {
            if (rewardList[i].specialOption != null) continue;

            // id ������ �������̴� �װ��� ���� optionmanager���Լ� �����´�. 
            if(OptionManager.instance == null)
            {
                Debug.Log("�ɼǸŴ����� �����ϴ�.");
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

    // �÷��̾�鿡�� ���ڵ� �����ϱ�
    public void ApplyRecordToPlayers(List<CharacterController> players)
    {
        for (int i = selectRecordInfos.Count - 1; i >= 0; i--)
        {
            RecordInfo record = selectRecordInfos[i];
            // ?? specialOption �� null �̶�� �����ʿ� ������ ���϶�� �� 
            record.specialOption ??= GetSpecialOptionToRecordInfo(record.specialOptionID);

            foreach (var player in players)
            {
                if (player.MyPlayer == null) continue;
                
                // �Ʒ� �ڷ�ƾ ���� 
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

        // todo ��Ÿ���� ���� �� ó�� ���� �߰� �ڸ� 
    }

    // ������Ʈ ������ �����ϴ� �Լ� 
    // ������ ���ڵ���� ��ȸ�ϸ� ��Ÿ���� ��� 
    public void UpdateRecordTiemers()
    {
        for (int i = selectRecordInfos.Count - 1; i >= 0; i--)
        {
            RecordInfo record = selectRecordInfos[i];
            // ?? specialOption �� null �̶�� �����ʿ� ������ ���϶�� �� 
            record.specialOption ??= GetSpecialOptionToRecordInfo(record.specialOptionID);

            if (record.specialOption.coolTime <= 0)
            {

            }

        }
    }
    // ������ ���ڵ���� �� �ʱ�ȭ�Ѵ�. 
    public void ClearRecords()
    {
        selectRecordInfos.Clear();  
    }
}
