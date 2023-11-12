using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : Skill
{
    public float baseDamage = 0;
    [SerializeField]
    private float damage = 0;

    [SerializeField]
    private int cost = 0;

    [SerializeField]
    private float speed = 0f;

    [SerializeField]
    private float coolTime = 0f;
    private bool isCoolDown = true;

    [SerializeField]
    private float castTime = 0f;

    [SerializeField]
    private GameObject skillPrefab = null;


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
        set { isCoolDown = value; }
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

    public override void Use()
    {

    }

    public override void Use(WheelerController controller)
    {
        if (controller == null) return;
        base.Use();
        var damage = CalcSkillDamage(controller);
        controller.UseSkill(this, damage);
    }

    public void CoolTimeReset()
    {
        MyCoolDown = true;
    }


    public override void UpgradeSkill()
    {
        base.UpgradeSkill();
        MySkillLevel += 1;
        CalcUpgradeCost();
        MyDamage = baseDamage + (skillLevel - 1) *  coefficient;
    }


    public int CalcSkillDamage(WheelerController controller)
    {
        if (controller == null || controller.MyPlayer == null) return 0;

        int result;

        result = Mathf.RoundToInt((float)controller.MyPlayer.MyStat.totalATK
            * MyDamage);

        return result;
    }

}

