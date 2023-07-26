using System.Collections;
using System.Collections.Generic;
using UnityEngine;





// 게임내 사용될 메모리 클래스
[System.Serializable]
public class MemoryInfo
{
    public int id;          // 식별 id
    public string name;     // 메모리 이름
    public string description; // 메모리 설명 
    public int grade;       // 메모리 등급
    public int optionID;
    public SpecialOption specialOption; // 메모리 효과 

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

// Memory 정보를 가진 Json 클래스
[System.Serializable]
public class MemoryInfoJson
{
    public int id;
    public string namekeycode;
    public string description;
    public int grade;
    public int optionID;
}

// MemoryInfoJson 클래스를 리스트로 담고 있는 클래스 
[System.Serializable]
public class MemoryInfoJsonAllData
{
    public MemoryInfoJson[] memoryInfoJson;
}



// 게임 내 출현하는 메모리 관리하는 매니저

public class MemoryManager : MonoBehaviour
{

    public static MemoryManager instance; 

    [Header("메모리 정보 JSON 데이터")]
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


    // MemoryInfo 초기화 
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

}
