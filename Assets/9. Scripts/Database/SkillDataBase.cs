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

    [SerializeField] private Skill[] activeSkills = null;
    [SerializeField] private Skill[] passiveSkills = null;

    public List<Skill> activeSkillList = new List<Skill>();
    public List<Skill> passiveSkillList = new List<Skill>();

    [SerializeField] private SkillToolTip skillToolTip = null;

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


    public Skill[] GetActiveSkills()
    {
        return activeSkills;
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


    public Skill[] GetPassiveSkills()
    {
        return passiveSkills;
    }

    public void SetSkill(PlayerControl target)
    {
        CharStat tempStat = new CharStat(1, 10, 10, 10, 100, 100, 10);
        Character tempPlayer = new Character();
        tempPlayer.MyStat = tempStat;
        target.MyPlayer = tempPlayer;

        tempPlayer.SetSkill(activeSkills[2], 0, false);
        
        skilbtn.playerControl = target;
        skilbtn.SetSkill(0);
    }


  
    
}
