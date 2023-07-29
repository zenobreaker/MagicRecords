using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





// ���ӳ� ���� �޸� Ŭ����
[System.Serializable]
public class MemoryInfo
{
    public int id;          // �ĺ� id
    public string name;     // �޸� �̸�
    public string description; // �޸� ���� 
    public int grade;       // �޸� ���
    public int optionID;
    public string spritePath;   // �޸� ��������Ʈ ���
    public SpecialOption specialOption; // �޸� ȿ�� 

    public MemoryInfo(int id, string name, string description, int grade, int optionID)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.grade = grade;
        this.optionID = optionID;
    }

    public MemoryInfo(int id, string name, string description, int grade,
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
public class MemoryInfoJson
{
    public int id;
    public string namekeycode;
    public string description;
    public int grade;
    public int optionID;
}

// MemoryInfoJson Ŭ������ ����Ʈ�� ��� �ִ� Ŭ���� 
[System.Serializable]
public class MemoryInfoJsonAllData
{
    public MemoryInfoJson[] memoryInfoJson;
}



// ���� �� �����ϴ� �޸� �����ϴ� �Ŵ���

public class MemoryManager : MonoBehaviour
{

    public static MemoryManager instance; 

    [Header("�޸� ���� JSON ������")]
    public TextAsset memoryInfoJsonData;

    private MemoryInfoJsonAllData memoyInfoJsonAlldata;


    public Dictionary<int, MemoryInfo> memoryInfoDictionary = new Dictionary<int, MemoryInfo>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

        InitializeMemoryInfo();
    }


    // MemoryInfo �ʱ�ȭ 
    public void InitializeMemoryInfo()
    {
        memoyInfoJsonAlldata = JsonUtility.FromJson<MemoryInfoJsonAllData>(memoryInfoJsonData.text);
        if (memoyInfoJsonAlldata == null ||
            memoyInfoJsonAlldata.memoryInfoJson == null)
            return; 

        foreach(MemoryInfoJson memInfoJson in memoyInfoJsonAlldata.memoryInfoJson)
        {
            if(memInfoJson == null) continue;

            MemoryInfo memoryInfo = new MemoryInfo(memInfoJson.id, memInfoJson.namekeycode, 
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
    public MemoryInfo GetMemoryInfoByID(int id)
    {
        return memoryInfoDictionary[id];
    }

    // �޸𸮸� �������� �޴� �Ű�������ŭ ��ȯ���ִ� �Լ� 
    public List<MemoryInfo> GetRandomRewardMemory(int count)
    {
        List<MemoryInfo> rewardList = new List<MemoryInfo>();


        // 1. ������ �޸� id�� ��ųʸ����� �ߺ� ���� �����´�. 
        List<int> keys= new List<int>(memoryInfoDictionary.Keys);
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
