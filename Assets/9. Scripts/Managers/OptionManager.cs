using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


// �ɼ� �߻� ���� Ÿ�� 
public enum ConditionType
{
    NONE = 0,  
    DURATION,    // �� ���Ǿ��� �����ð����� ȿ�� ���� 
    ADJOIN,      // Ư�� ���� ���� ���� �ߵ�
    TRY_ATTACK,   // ���� �� �ߵ�
    TRY_HIT,     // �ǰ� �� �ߵ�
    COST,       // �ڽ�Ʈ�� ������ ��� �ߵ�
}

public enum OptionType
{
    NONE = 0, 
    BUFF,
    DEBUFF,
}

// ���ӳ� ���� Ư���� ȿ�� Ŭ����
[System.Serializable]
public class SpecialOption
{   
    public int effectID;        // ȿ�� ID
    public string keycode;      // ���� Ű�ڵ尪
    public ConditionType conditionType; //ȿ�� �߻� ����
    public string effectName;   // ȿ�� �̸�
    public OptionType optionType;   // ȿ�� Ÿ��
    public string description;  // ȿ�� ���� 
    public float value;         // ȿ�� ��ġ
    public float duration;      // ȿ�� ���� �ð�
    public AbilityType abilityType; // ȿ�� Ÿ�� 
    public bool isPercentage;
    public float coolTime;

    public SpecialOption(int effectID, string effectName, string description,
        OptionType optionType,
        ConditionType conditionType = ConditionType.NONE, 
         float duration = 0, AbilityType abilityType = AbilityType.NONE,
         float value = 0.0f, int isPercentage = 1)
    {
        this.effectID = effectID;      
        this.effectName = effectName;
        this.description = description;
        this.optionType = optionType;
        this.conditionType = conditionType;
        this.value = value;
        this.duration = duration;
        this.abilityType = abilityType;
        this.isPercentage = isPercentage == 1 ? true : false;
    }

    public void SetCoolTime()
    {
        coolTime = duration; 
    }

    public SpecialOption Clone()
    {
        return new SpecialOption(effectID, effectName, description, optionType, conditionType,
            duration, abilityType, value, isPercentage == true? 1: 0);
    }
}



[System.Serializable]
public class SpecialOptionJson
{
    public int id;
    public string keycode;
    public string namekeycode;
    public string description;
    public OptionType optionType;
    public int conditionType;
    public float conditionValue;
    public int abilityType;
    public float value;
    public int isPercentage;
}


[System.Serializable]
public class SpecialOptionInfoJsonAllData
{
    public SpecialOptionJson[] specialOptionJson;
}


// Option ������ ���� json Ŭ���� 
[System.Serializable] 
public class OptionInfoJson
{
    public int id;
    public string keycode;
    public string description;
    public int abilityType;
    public float value;
}


//OptionInfoJson Ŭ������ ����Ʈ�� ��� �ִ� Ŭ���� 
[System.Serializable]
public class OptionInfoJsonAllData
{
    public OptionInfoJson[] effectInfoJsons;
}



// ���� ���� ȿ������ �����ϴ� �Ŵ��� 
public class OptionManager : MonoBehaviour
{
    public static OptionManager instance;

    [Header("ȿ�� ���� JSON ������")]
    public TextAsset optionInfoJsonData;
    [Header("Ư���� ȿ�� ���� JSON ������")]
    public TextAsset specialOptionJsonData; 

    private OptionInfoJsonAllData optionInfoJsonAllData;

    private SpecialOptionInfoJsonAllData specialOptionInfoAllData; 

    // Ư���� ȿ������ ����� ��ųʸ� keycode class
    public Dictionary<string, SpecialOption> specialOptionsDictionary = new Dictionary<string, SpecialOption>();

    // �Ϲ� ȿ������ ����� ��ųʸ�
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

            // data���� conditionType ����
            ConditionType type = (ConditionType)data.conditionType;
            AbilityType abilityType = (AbilityType)data.abilityType;

            // Ŭ���� ����
            SpecialOption specialOption = new SpecialOption(data.id,
                data.namekeycode, data.description, data.optionType, 
                type, data.conditionValue,
                abilityType, data.value, data.isPercentage);

            // 2023.09.14 keycode �߰�
            specialOption.keycode = data.keycode;

            // ��ųʸ��� �� �߰�
            specialOptionsDictionary.Add(data.keycode.ToString(), specialOption);
        }
    }

    // id ���� ���� ����� �ɼ��� ��ȯ
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


    // keycode ������ ����� �ɼ� ��ȯ
    public SpecialOption GetSpecialOptionByKeycode(string keycode)
    {
        if (keycode == "") return null;

        if(specialOptionsDictionary.ContainsKey(keycode))
        {
            return specialOptionsDictionary[keycode].Clone();
        }

        return null;
    }
}
