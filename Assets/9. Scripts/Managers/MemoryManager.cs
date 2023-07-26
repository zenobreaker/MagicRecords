using System.Collections;
using System.Collections.Generic;
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

}
