using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



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
    public ActiveSKillDataJson[] activeSKillDataJsons;
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
    public PassiveSkillDataJson[] passiveSkillDataJsons;
}



public class SkillDataBase : MonoBehaviour
{
    [SerializeField] private Skill[] activeSkills = null;
    [SerializeField] private Skill[] passiveSkills = null;

    public List<Skill> activeSkillList = new List<Skill>();
    public List<Skill> paissveSkilList = new List<Skill>();

    [SerializeField] private SkillToolTip skillToolTip = null;

    [SerializeField] ActionButton skilbtn = null;

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

    //
    public void InitSkillData()
    {
        passiveSkillDataJsonAllData = JsonUtility.FromJson<PassiveSkillDataJsonAllData>(activeSkillDataJosn.text);
        activeSkillDataJsonAllData = JsonUtility.FromJson<ActiveSkillDataJsonAllData>(activeSkillDataJosn.text);
        skillDataJsonAllData = JsonUtility.FromJson<SkillDataJsonAllData>(skillDataJson.text);
        if (skillDataJsonAllData == null || activeSkillDataJsonAllData == null ||
            passiveSkillDataJsonAllData == null) return; 

        foreach(SkillDataJson skillDataJson in skillDataJsonAllData.skillDataJson)
        {
            if (skillDataJson == null) continue;

            if (skillDataJson.isPassive == 1)
            {
                foreach (PassiveSkillDataJson passiveSkillData in passiveSkillDataJsonAllData.passiveSkillDataJsons)
                {
                    if (passiveSkillData == null || passiveSkillData.activation == 0) continue; 

                    // 적당히 패시브 스킬을 찾는다. 
                    if(skillDataJson.keycode.Equals(passiveSkillData.skillKeycode))
                    {
                        Skill passiveSkill = new Skill();
                        passiveSkill.MyName = "name_" + skillDataJson.keycode;
                        passiveSkill.CallSkillName = skillDataJson.callName;
                        passiveSkill.MySkillMaxLevel = passiveSkillData.maxLevel;
                        passiveSkill.leadingSkillList = ConvertListFormString(passiveSkillData.leadingSkillList);
                        passiveSkill.coefficient = passiveSkillData.coefficient;
                        paissveSkilList.Add(passiveSkill);
                    }
                }
            }
            else
            {
                foreach(ActiveSKillDataJson activeSKillData in activeSkillDataJsonAllData.activeSKillDataJsons)
                {
                    if(activeSKillData == null || activeSKillData.activation == 0) continue;

                    if(skillDataJson.keycode.Equals(activeSKillData.skillKeycode))
                    {
                        Skill activeSkill = new Skill();
                        activeSkill.MyName = "name_" + skillDataJson.keycode;
                        activeSkill.CallSkillName = skillDataJson.callName;
                        activeSkill.MySkillMaxLevel = activeSKillData.maxLevel;
                        activeSkill.leadingSkillList = ConvertListFormString(activeSKillData.leadingSkillList);
                        activeSkill.coefficient = activeSKillData.coefficient;
                        activeSkill.MyDamage = activeSKillData.baseDamage;
                        activeSkill.hitCount = activeSKillData.hitCount;
                        activeSkill.SkillCost = activeSKillData.cost;
                        activeSkill.MyCoolTime = activeSKillData.baseCoolTime;
                        activeSkill.bonusOptionList = ConvertListFormString(activeSKillData.bonusOptionList);
                        activeSkill.bonusSpecialOptionList = ConvertListFormString(activeSKillData.bonusSpecialOptionList);
                        activeSkillList.Add(activeSkill);
                    }
                }
            }
        }
    }


    // 툴팁 보이기 
    public void ShowSkillToolTip(Skill _skill)
    {
        skillToolTip.ShowToolTip(_skill);
    }

    // 툴팁 숨기기 
    public void HideSkillToolTip()
    {
        skillToolTip.HideToolTip();
    }

    public Skill[] GetActiveSkills()
    {
        return activeSkills;
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

        Debug.Log("스킬 정산 " + activeSkills[0].MyName);
        tempPlayer.SetSkill(activeSkills[2], 0, false);
        Debug.Log("스킬 정산 " + tempPlayer.MySkills[0].MyName);
        skilbtn.playerControl = target;
        skilbtn.SetSkill(0);
    }


  
    
}
