using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SkillDataJson
{
    public int id;
    public int userID;
    public string keycode;
    public string callName;
    public string objectName;
    public int isPassive;
    public int activation;
}

[System.Serializable]
public class SkillDataJsonAllData
{
    public SkillDataJson[] skillDataJson;
}


[System.Serializable]
public class ActiveSKillDataJson
{
    public int id;
    public string skillKeycode;
    public int maxLevel;
    public float baseDamage;
    public float coefficient;
    public int hitCount;
    public int cost;
    public float baseCoolTime;
    public float decreaseTimeValue;
    public string bonusOptionList;
    public string bonusSpecialOptionList;
    public string leadingSkillList;
    public int activation; 
}

[System.Serializable]
public class ActiveSkillDataJsonAllData
{
    public ActiveSKillDataJson[] activeSkillDataJson;
}

[System.Serializable]
public class PassiveSkillDataJson
{
    public int id;
    public string skillKeycode;
    public int maxLevel;
    public string bonusOptionList;
    public string enhanceSkillTargetList;
    public string leadingSkillList;
    public float coefficient;
    public int activation; 
}

[System.Serializable]
public class PassiveSkillDataJsonAllData
{
    public PassiveSkillDataJson[] passiveSkillDataJson;
}



public class SkillDataBase : MonoBehaviour
{
    public static SkillDataBase instance; 

    public List<Skill> activeSkillList = new List<Skill>();
    public List<Skill> passiveSkillList = new List<Skill>();

    [SerializeField] ActionButton skilbtn = null;

    public int baseUpgradeCost = 5000;

    public TextAsset skillDataJson;
    public TextAsset activeSkillDataJosn;
    public TextAsset passiveSkillDataJson;


    private SkillDataJsonAllData skillDataJsonAllData;
    private ActiveSkillDataJsonAllData activeSkillDataJsonAllData;
    private PassiveSkillDataJsonAllData passiveSkillDataJsonAllData;


    public List<string> ConvertListFormString(string targetStrring)
    {
        List<string> resultList = new List<string>();

        var split = targetStrring.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            string targetID = split[i];
            resultList.Add(targetID);
        }

        return resultList;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this; 
    }

    private void Start()
    {
        InitSkillData();
    }


    public List<int> GetSkillUpgradeCost(int maxLevel)
    {
        List<int> list = new List<int>();

        for (int i = 1; i <= maxLevel; i++)
        {
            list.Add(baseUpgradeCost * i);
        }

        return list; 
    }

    private string GetSkillName(string skillName)
    {
        return LanguageManager.Instance.GetLocaliztionValue(skillName);
    }
    
    // keycode 값을 통해서 스킬 스프라이트를 찾는다. 
    private Sprite GetSkillSprite(string keycode)
    {
        string path = "skill_" + keycode;
        Sprite skillImage = Resources.Load<Sprite>("Skill/" + path);
        return skillImage;
    }

    //
    public void InitSkillData()
    {
        passiveSkillDataJsonAllData = JsonUtility.FromJson<PassiveSkillDataJsonAllData>(passiveSkillDataJson.text);
        activeSkillDataJsonAllData = JsonUtility.FromJson<ActiveSkillDataJsonAllData>(activeSkillDataJosn.text);
        skillDataJsonAllData = JsonUtility.FromJson<SkillDataJsonAllData>(skillDataJson.text);
        if (skillDataJsonAllData == null || activeSkillDataJsonAllData == null ||
            passiveSkillDataJsonAllData == null) return; 

        foreach(SkillDataJson skillDataJson in skillDataJsonAllData.skillDataJson)
        {
            if (skillDataJson == null) continue;

            if (skillDataJson.isPassive == 1)
            {
                foreach (PassiveSkillDataJson passiveSkillData in passiveSkillDataJsonAllData.passiveSkillDataJson)
                {
                    if (passiveSkillData == null || passiveSkillData.activation == 0) continue; 

                    // 적당히 패시브 스킬을 찾는다. 
                    if(skillDataJson.keycode.Equals(passiveSkillData.skillKeycode))
                    {
                        PassiveSkill passiveSkill = new PassiveSkill();
                        passiveSkill.id = skillDataJson.id;
                        passiveSkill.skillType = SkillType.PASSIVE;
                        passiveSkill.userID = skillDataJson.userID;
                        passiveSkill.keycode = skillDataJson.keycode;
                        passiveSkill.MyName = GetSkillName("name_" + skillDataJson.keycode);
                        passiveSkill.CallSkillName = skillDataJson.callName;
                        passiveSkill.MySkillMaxLevel = passiveSkillData.maxLevel;
                        passiveSkill.leadingSkillList = ConvertListFormString(passiveSkillData.leadingSkillList);
                        passiveSkill.coefficient = passiveSkillData.coefficient;
                        passiveSkill.baseCost = baseUpgradeCost;
                        passiveSkill.CalcUpgradeCost();
                        passiveSkill.MyIcon = GetSkillSprite(skillDataJson.keycode);
                        passiveSkill.SkillOptionSet(ConvertListFormString(passiveSkillData.bonusOptionList));
                        passiveSkillList.Add(passiveSkill);
                    }
                }
            }
            else
            {
                foreach(ActiveSKillDataJson activeSKillData in activeSkillDataJsonAllData.activeSkillDataJson)
                {
                    if(activeSKillData == null || activeSKillData.activation == 0) continue;

                    if(skillDataJson.keycode.Equals(activeSKillData.skillKeycode))
                    {
                        ActiveSkill activeSkill = new ActiveSkill();
                        activeSkill.id = skillDataJson.id;
                        activeSkill.skillType = SkillType.ACTIVE;
                        activeSkill.userID = skillDataJson.userID;
                        activeSkill.keycode = skillDataJson.keycode;
                        activeSkill.MyName = GetSkillName("name_" + skillDataJson.keycode);
                        activeSkill.CallSkillName = skillDataJson.callName;
                        activeSkill.MyIcon = GetSkillSprite(skillDataJson.keycode);
                        activeSkill.MySkillMaxLevel = activeSKillData.maxLevel;
                        activeSkill.leadingSkillList = ConvertListFormString(activeSKillData.leadingSkillList);
                        activeSkill.coefficient = activeSKillData.coefficient;
                        activeSkill.baseDamage = activeSKillData.baseDamage;
                        activeSkill.hitCount = activeSKillData.hitCount;
                        activeSkill.SkillCost = activeSKillData.cost;
                        activeSkill.MyCoolTime = activeSKillData.baseCoolTime;
                        activeSkill.baseCost = baseUpgradeCost;
                        activeSkill.CalcUpgradeCost();
                        activeSkill.bonusOptionList = ConvertListFormString(activeSKillData.bonusOptionList);
                        activeSkill.SkillOptionSet(ConvertListFormString(activeSKillData.bonusSpecialOptionList));
                        activeSkillList.Add(activeSkill);
                    }
                }
            }
        }
    }



    // id랑 맞는 스킬 리스트를 가져온다. 
    public List<Skill> GetActiveSkillListFromID(int id)
    {
        List<Skill> skillList = new List<Skill>();
        
        foreach(var skill in activeSkillList)
        {
            if(skill == null || id != skill.userID)
            {
                continue; 
            }

            skillList.Add(skill);
        }

        return skillList;
    }

    public List<Skill> GetPassiveSkillListFromID(int id)
    {
        List<Skill> skillList = new List<Skill>();

        foreach (var skill in passiveSkillList)
        {
            if (skill == null || id != skill.userID)
            {
                continue;
            }

            skillList.Add(skill);
        }

        return skillList;
    }


    //public void SetSkill(PlayerControl target)
    //{
    //    CharStat tempStat = new CharStat(1, 10, 10, 10, 100, 100, 10);
    //    Character tempPlayer = new Character();
    //    tempPlayer.MyStat = tempStat;
    //    target.MyPlayer = tempPlayer;

    //    tempPlayer.SetSkill(activeSkills[2], 0, false);
        
    //    skilbtn.controller = target;
    //    skilbtn.SetSkill(0);
    //}


    // 스킬 정보를 받으면 스킬의 설명문을 반환해주는 함수
    public string GetSkillDesc(Skill skill)
    {
        string result = "";

        if (LanguageManager.Instance == null) return result;

        // 스킬에는 고유 효과나 부가 효과로 인한 여러 가지 기능들이 있다. 
        // 그것들은 bonusOption으로 처리하기 때문에 LangaugeManager에게 잘 전달 해야한다. 
        result = LanguageManager.Instance.GetLocaliztionValue(skill.keycode);

        // 스킬의 위력 값 - 액티브 
        float value = 0.0f; 

        if(skill is ActiveSkill)
        {
            value = (skill as ActiveSkill).MyDamage;
        }

        result = LanguageManager.Instance.ReplaceVariables(result, new Dictionary<string, object>
        {
            {"SKILL_POWER_1", value},
        }, true);

        // 스킬이 갖고 있는 옵션들 
        List<SpecialOption> optionList = skill.bonusSpecialOptionList;

        int count = 1; 
        foreach (var option in optionList)
        {
            if (option == null) continue;

            string durationKey = "OPTION_DURATION_" + count;
            string durationValue = "OPTION_VALUE_" + count;

            result = LanguageManager.Instance.ReplaceVariables(result, new Dictionary<string, object>
            {
                {durationKey, option.duration},
                {durationValue, option.value * 100.0f}, // 스킬 계수는 소수점이므로 표기는 백분율로 보이도록 
            }, option.isPercentage);

            count++;
        }

        // todo 스킬 계수가 리스트라면 리스트로 순회하도록 스킬을 수정할 필요가 있을 수도 
        // 스킬의 고유한 효과 (패시브)
        float passiveOptionValue = 0.0f;
        passiveOptionValue = skill.coefficient * skill.MySkillLevel * 100.0f; 
        result = LanguageManager.Instance.ReplaceVariables(result, new Dictionary<string, object>
        {
            {"UNIQUE_VALUE_1", passiveOptionValue }
        }, true);


        return result; 
    }
    

    // 스킬 키코드를 받으면 액티브 스킬을 반환하는 함수
    public Skill GetActiveSkillBySkillKeycode(string keycode)
    {
        if (activeSkillList == null || keycode == null) return null; 

        foreach(var skill in activeSkillList)
        {
            if(skill.keycode == keycode)
            {
                return skill; 
            }
        }

        return null; 
    }

    public Skill GetPassiveSkillBySkillKeycode(string keycode)
    {
        if (passiveSkillList == null) return null;

        foreach (var skill in passiveSkillList)
        {
            if (skill.keycode == keycode)
            {
                return skill;
            }
        }

        return null;
    }


    public void SetActiveSkill(string keycode, int level, bool isChain)
    {
        if (activeSkillList == null) return;

        var activeSkill = activeSkillList.Find(skill => skill.keycode == keycode);
        if (activeSkill == null)
            return;

        activeSkill.MySkillLevel = level;
        activeSkill.IsChain = isChain;
        activeSkill.CalcUpgradeCost(); 
    }

    public void SetPassiveSkill(string keycode, int level)
    {
        if (passiveSkillList == null) return; 

        var passiveSkill = passiveSkillList.Find(skill => skill.keycode == keycode);
        if (passiveSkill == null)
            return;

        passiveSkill.MySkillLevel = level;
        passiveSkill.CalcUpgradeCost();
    }
}
