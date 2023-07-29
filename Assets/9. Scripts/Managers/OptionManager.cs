using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 옵션 발생 조건 타입 
public enum ConditionType
{
    NONE = 0,  
    DURATION,    // 별 조건없이 일정시간동안 효과 적용 
    ADJOIN,      // 특정 범위 내에 오면 발동
    ATTACKING,   // 공격 시 발동
    HITTING,     // 피격 시 발동
    COST,       // 코스트를 지불할 경우 발동
}

// 게임내 사용될 특별한 효과 클래스
[System.Serializable]
public class SpecialOption
{   
    public int effectID;        // 효과 ID
    public ConditionType conditionType; //효과 발생 조건
    public string effectName;   // 효과 이름
    public string description;  // 효과 설명 
    public float value;         // 효과 수치
    public float duration;      // 효과 유지 시간

    public SpecialOption(int effectID, string effectName, string description,
         float value, ConditionType conditionType = ConditionType.NONE, float duration = 0)
    {
        this.effectID = effectID;      
        this.effectName = effectName;
        this.description = description;
        this.conditionType = conditionType;
        this.value = value;
        this.duration = duration;
    }
}



[System.Serializable]
public class SpecialOptionJson
{
    public int id;
    public string namekeycode;
    public string description;
    public int conditionType;
    public float conditionValue;
    public float value; 
}


[System.Serializable]
public class SpecialOptionInfoJsonAllData
{
    public SpecialOptionJson[] specialOptionJson;
}


// Option 정보를 가진 json 클래스 
[System.Serializable] 
public class OptionInfoJson
{
    public int id;
    public string keycode;
    public float value;
    public string description;
}


//OptionInfoJson 클래스를 리스트로 담고 있는 클래스 
[System.Serializable]
public class OptionInfoJsonAllData
{
    public OptionInfoJson[] effectInfoJsons;
}



// 게임 내에 효과들을 관리하는 매니저 
public class OptionManager : MonoBehaviour
{
    public static OptionManager instance;

    [Header("효과 정보 JSON 데이터")]
    public TextAsset optionInfoJsonData;
    [Header("특별한 효과 정보 JSON 데이터")]
    public TextAsset specialOptionJsonData; 

    private OptionInfoJsonAllData optionInfoJsonAllData;

    private SpecialOptionInfoJsonAllData specialOptionInfoAllData; 

    // 특별한 효과들이 담겨진 딕셔너리 keycode class
    public Dictionary<string, SpecialOption> specialOptionsDictionary = new Dictionary<string, SpecialOption>();

    // 일반 효과들이 담겨진 딕셔너리
    public Dictionary<string, SpecialOption> specialOptionsDictionary2 = new Dictionary<string, SpecialOption>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

        InitializeSpecialOptionInfo();
    }


    public void InitializeSpecialOptionInfo()
    {
        specialOptionInfoAllData = JsonUtility.FromJson<SpecialOptionInfoJsonAllData>(specialOptionJsonData.text);
        if (specialOptionInfoAllData == null ||
            specialOptionInfoAllData.specialOptionJson == null) return;

        foreach(SpecialOptionJson data in specialOptionInfoAllData.specialOptionJson)
        {
            if (data == null) continue;

            // data에서 conditionType 수정
            ConditionType type = (ConditionType)data.conditionType;
            float conditionValue = data.value;

            // 클래스 생성
            SpecialOption specialOption = new SpecialOption(data.id,
                data.namekeycode, data.description, data.value, type, conditionValue);

            // 딕셔너리에 값 추가
            specialOptionsDictionary.Add(data.namekeycode.ToString(), specialOption);
        }
    }

    // id 값을 통해 스페셜 옵션을 반환
    public SpecialOption GetSpecialOption(int id)
    {
        if (id < 0) return null;

        foreach(var option in specialOptionsDictionary)
        {
            if (option.Value == null) continue;

            if(option.Value.effectID == id)
            {
                return option.Value;
            }
        }

        return null; 
    }

}
