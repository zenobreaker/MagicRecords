using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �ɼ� �߻� ���� Ÿ�� 
public enum ConditionType
{
    NONE = 0,  
    DURATION,    // �� ���Ǿ��� �����ð����� ȿ�� ���� 
    ADJOIN,      // Ư�� ���� ���� ���� �ߵ�
    ATTACKING,   // ���� �� �ߵ�
    HITTING,     // �ǰ� �� �ߵ�
    COST,       // �ڽ�Ʈ�� ������ ��� �ߵ�
}

// ���ӳ� ���� Ư���� ȿ�� Ŭ����
[System.Serializable]
public class SpecialOption
{   
    public int effectID;        // ȿ�� ID
    public ConditionType conditionType; //ȿ�� �߻� ����
    public string effectName;   // ȿ�� �̸�
    public string description;  // ȿ�� ���� 
    public float value;         // ȿ�� ��ġ
    public float duration;      // ȿ�� ���� �ð�

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


// Option ������ ���� json Ŭ���� 
[System.Serializable] 
public class OptionInfoJson
{
    public int id;
    public string keycode;
    public float value;
    public string description;
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
            float conditionValue = data.value;

            // Ŭ���� ����
            SpecialOption specialOption = new SpecialOption(data.id,
                data.namekeycode, data.description, data.value, type, conditionValue);

            // ��ųʸ��� �� �߰�
            specialOptionsDictionary.Add(data.namekeycode.ToString(), specialOption);
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

}
