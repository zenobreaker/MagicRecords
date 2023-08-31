using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { ACTIVE = 1, PASSIVE,};

[Serializable]
public class Skill : IUseable, IMoveable
{

    public int id; 

    [SerializeField]
    private string name = null;

    [SerializeField] string skillName = null;

    [SerializeField] private SkillType skillType;

    [SerializeField]
    private int skillLevel = 0;
    [SerializeField] int skillMaxLevel = 0;

    [SerializeField]
    private float damage = 0;

    public float coefficient = 1; // 스킬 레벨 증가시 오르는 대미지의 계수값

    public int hitCount = 0;    // 타격 횟수 - 각 스킬오브젝트별 개별 타수

    [SerializeField]
    private int cost = 0;

    [SerializeField]
    private Sprite icon = null;

    [SerializeField]
    private float speed = 0f;

    [SerializeField]
    private float coolTime = 0f;
    private bool isCoolDown = true;

    [SerializeField]
    private float castTime = 0f;

    [SerializeField]
    private GameObject skillPrefab = null;

    public List<string> bonusOptionList = new List<string>();
    public List<string> bonusSpecialOptionList = new List<string>();
    public List<string> leadingSkillList = new List<string>();

    public List<int> upgradeCost = new List<int>();

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

    public float MyDamage
    {
        get { return damage; }
        set { damage = value; }
    }

    public int SkillCost
    {
        get { return cost; }
        set { cost = value; }
    }

    public Sprite MyIcon
    {
        get { return icon; }
        set { icon = value; }
    }

    public float MySpeed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float MyCoolTime
    {
        get { return coolTime; }
        set { coolTime = value; }
    }
    public bool MyCoolDown
    {
        get { return isCoolDown; }
        set {  isCoolDown = value; }
    }

    public float MyCastTime
    {
        get { return castTime; }
        set { castTime = value; }
    }
   
    public GameObject MySkillPrefab
    {
        get { return skillPrefab; }
        set { skillPrefab = value; }
    }


    public int CalcSkillDamage(CharacterController controller)
    {
        if (controller == null || controller.MyPlayer == null) return 0;
        
        int result;

        result =  Mathf.RoundToInt((float)controller.MyPlayer.MyStat.totalATK 
            * skillLevel * coefficient);

        return result;
    }

    public void Use()
    {
       // SkillAction.MyInstance.ActionSkill(MyName);
    }

    public void Use(CharacterController controller)
    {
        if (controller == null) return;
        var damage = CalcSkillDamage(controller);
        controller.UseSkill(this, damage);
    }

    public void CoolTimeReset()
    {
        MyCoolDown = true;
    }


    public void UpgradeSkill()
    {
        MySkillLevel += 1;
        MyDamage += coefficient;
    }


    public Skill DeepCopy()
    {
        Skill returnSkill = new Skill();

        // 스킬명
        returnSkill.MyName = this.MyName;
        // 스킬 호출명
        returnSkill.CallSkillName = CallSkillName;
        // 스킬 레벨
        returnSkill.MySkillLevel = MySkillLevel;
        // 스킬의 최대 레벨 
        returnSkill.MySkillMaxLevel = this.MySkillMaxLevel;
        // 스킬의 프리팹(이건 이제 필요없다..)
        returnSkill.MySkillPrefab = this.MySkillPrefab;
        // 스킬의 아이콘
        returnSkill.MyIcon = this.MyIcon;
        // 스킬의 캐스팅 시간
        returnSkill.MyCastTime = this.MyCastTime;
        // 스킬의 소비 코스트
        returnSkill.SkillCost = this.SkillCost;
        // 스킬의 쿨타임 여부
        returnSkill.MyCoolDown = this.MyCoolDown;
        // 스킬의 쿨타임
        returnSkill.MyCoolTime = this.MyCoolTime;
        //스킬의 데미지 비례
        returnSkill.MyDamage = this.MyDamage;
        // 스킬의 속도
        returnSkill.MySpeed = this.MySpeed;
        // 체인 여부 
        returnSkill.isChain = false;
        return returnSkill;
    }
}
