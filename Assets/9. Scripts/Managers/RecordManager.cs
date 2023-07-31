using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





// ���ӳ� ���� �޸� Ŭ����
[System.Serializable]
public class RecordInfo
{
    public int id;          // �ĺ� id
    public string name;     // �޸� �̸�
    public string description; // �޸� ���� 
    public int grade;       // �޸� ���
    public int optionID;
    public string spritePath;   // �޸� ��������Ʈ ���
    public SpecialOption specialOption; // �޸� ȿ�� 

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

// Memory ������ ���� Json Ŭ����
[System.Serializable]
public class RecordInfoJson
{
    public int id;
    public string namekeycode;
    public string description;
    public int grade;
    public int optionID;
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

    [Header("�޸� ���� JSON ������")]
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
                memInfoJson.description, memInfoJson.grade, memInfoJson.optionID);

            // �ɼǸŴ������� �����÷��� �ɼ��� �ִ��� �˻�
            if(OptionManager.instance != null)
            {
                if(OptionManager.instance.specialOptionsDictionary.ContainsKey(memInfoJson.namekeycode) == true)
                {
                    // �ش� info Ŭ������ �ɼ� Ŭ������ �־�д�.
                    memoryInfo.specialOption = OptionManager.instance.specialOptionsDictionary[memInfoJson.namekeycode];
                }
            }

            // ����Ʈ�� �߰��ϱ� 
            memoryInfoDictionary.Add(memInfoJson.id, memoryInfo);   
        }
    }

    // Ư�� ID ���� ������ �ش� �޸𸮸� ��ȯ�ϴ� �Լ�
    public RecordInfo GetRecordInfoByID(int id)
    {
        if (memoryInfoDictionary.ContainsKey(id) == false)
            return null; 

        return memoryInfoDictionary[id];
    }

    // �޸𸮸� �������� �޴� �Ű�������ŭ ��ȯ���ִ� �Լ� 
    public List<RecordInfo> GetRandomRewardMemory(int count)
    {
        List<RecordInfo> rewardList = new List<RecordInfo>();


        // 1. ������ �޸� id�� ��ųʸ����� �ߺ� ���� �����´�. 
        List<int> keys= new List<int>(memoryInfoDictionary.Keys);
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

            if(prevIndx == idx || rewardList.Contains(memoryInfoDictionary[keys[idx]]) == true)
            {
                continue; 
            }

            if(memoryInfoDictionary.ContainsKey(idx) == true)
            {
                // 2. ���� ����Ʈ�� ���� �޸𸮸� ������ �����Ѵ�.
                rewardList.Add(memoryInfoDictionary[idx]);
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

            int id = rewardList[i].optionID;

            rewardList[i].specialOption = OptionManager.instance.GetSpecialOption(id);
        }

        return rewardList;
    }

}
