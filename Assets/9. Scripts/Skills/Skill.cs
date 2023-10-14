using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { ACTIVE = 1, PASSIVE,};

[Serializable]
public class Skill : IUseable, IMoveable
{

    public int id;
    public int userID;  // 이 스킬을 사용하는 대상의 ID
    public string keycode; 

    [SerializeField]
    private string name = null;

    [SerializeField] string skillName = null;

    public SkillType skillType;

    [SerializeField]
    protected int skillLevel = 0;
    [SerializeField] int skillMaxLevel = 0;

    public float coefficient = 1; // 스킬 레벨 증가시 오르는 대미지의 계수값

    [SerializeField]
    private Sprite icon = null;

    public int hitCount = 0;    // 타격 횟수 - 각 스킬오브젝트별 개별 타수

    // todo 아래 옵션 리스트는 삭제 예정 bonusSpecialOptionList ->bonusOptionList  명명
    public List<string> bonusOptionList = new List<string>();
    public List<SpecialOption> bonusSpecialOptionList = new List<SpecialOption>();
    public List<string> leadingSkillList = new List<string>();

    public int baseCost = 0;
    public int upgradeCost = 0;

    public string skillSubDesc;

    [TextArea]
    public string skillDesc;

    [SerializeField]
    public bool isChain = false;
    public bool IsChain{ 
        get { return isChain; }
        set { isChain = value; }
    }

    public string MyName
    {
        get { return name; }
        set { name = value; }
    }

    public string CallSkillName
    {
        get { return skillName; }
        set { skillName = value; }
    }

    public int MySkillLevel
    {
        get { return skillLevel; }
        set { skillLevel = value; }
    }

    public int MySkillMaxLevel
    {
        get { return skillMaxLevel; }
        set { skillMaxLevel = value; }
    }
    public Sprite MyIcon
    {
        get { return icon; }
        set { icon = value; }
    }


    public int CalcUpgradeCost()
    {
        upgradeCost = baseCost * (skillLevel + 1);
        return upgradeCost;
    }


    public virtual void Use()
    {

    }

    public virtual void Use(WheelerController controller)
    {
        if (controller == null) return;
    }


    public virtual void UpgradeSkill()
    {
        MySkillLevel += 1;
        CalcUpgradeCost();
    }

    public void SkillOptionSet(List<string> optionKeycodeList)
    {
        foreach(var keycode in optionKeycodeList)
        {
            if (keycode == "") continue; 

            SpecialOption option = OptionManager.instance.GetSpecialOptionByKeycode(keycode);
            bonusSpecialOptionList.Add(option);
        }
    }
}
