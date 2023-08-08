using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharStat
{
    public float baseCritDamage = 150.0f;

    public MonsterGrade myGrade;
    public int level;
    // 통상 장비나 레벨업 같은 스탯이 적용되어 보이는 스탯들 
    public int totalATK;
    public int totalDEF;
    public float totalASPD;
    public int totalHP;
    public int totalMP;
    public int totalHPR;
    public int totalMPR;
    public int totalSPD;
    public float totalCritRate;
    public float totalCritDmg;


    // 기본적으로 갖고 있는 스탯 
    public int attack;
    public int defense;
    public float attackSpeed;
    public int hp;
    public int mp;
    public int hpRegen;
    public int mpRegen;
    public int speed;
    public float critRate;
    public float critDmg;


    public int exp;
    public int maxExp;
    public int giveChanceExp;

    public ExtraStat extraStat = new ExtraStat();

    public CharStat(CharStat _target)
    {
        Debug.Log("CharStat Deep Copy");
        attack = _target.attack;
        defense = _target.defense;
        attackSpeed = _target.attackSpeed;
        hp = _target.hp;
        mp = _target.mp;
        hpRegen = _target.hpRegen;
        mpRegen = _target.mpRegen;
        speed = _target.speed;
        critRate = _target.critRate;
        critDmg = _target.critDmg;

        ApplyOption(); 
    }

    public CharStat(int _level, int _atk, int _def, float _aspd, int _hp, int _mp, int _spd)
    {
        level = _level;
        attack = _atk;
        attackSpeed = _aspd;
        defense = _def;
        hp = _hp;
        mp = _mp;
        speed = _spd;

        ApplyOption();
    }

    public CharStat(MonsterGrade grade, int level, int atk, int def, float aspd, 
        int hp, int hpRegen, int mp, int mpRegen, int spd,
        float critRate, float critDmg)
    {
        this.myGrade = grade;
        this.level = level;
        this.attack = atk;
        this.attackSpeed = aspd;
        this.defense = def;
        this.hp = hp;
        this.hpRegen = hpRegen;
        this.mp = mp;
        this.mpRegen = mpRegen;
        this.speed = spd;
        this.critRate = critRate;
        this.critDmg = critDmg;

        ApplyOption();
    }

    //  캐릭터 성장 
    public void GrowUp(int _exp)
    {
        this.exp += _exp; 
        if(this.exp >= this.maxExp)
        {
            this.exp = 0;

            // 각 스탯별 능력치 증가 todo 계산식을 받아서 처리해보도록 하장
            this.level += level + 1;

            this.maxExp = maxExp + 10;
            //this.hp = hp + 10;
            //this.mp = mp + 10;
            //this.hpRecovery = hpRecovery + 1;
            //this.mpRecovery = mpRecovery + 1;
            //this.attack = attack + 5;
            //this.defense = defense + 5;

            ApplyOption(); 
        }
    }

    public void ApplyOption()
    {
        var resultAtk = 0;
        var resultDef = 0;
        var resultHP = 0;
        var resultHPR = 0;
        var resultMP = 0;
        var resultMPR = 0;
        var resultAspd = 0.0f;
        var resultSPD = 0;
        
        if (InfoManager.instance != null)
        {

            resultAtk = InfoManager.instance.GetGrowUpAttack(this.level, attack);
            resultDef = InfoManager.instance.GetGrowUpDefense(this.level, defense);
            resultHP = InfoManager.instance.GetGrowUpHP(this.level, hp);
            resultHPR = InfoManager.instance.GetGrowUpHPRecovery(this.level, hpRegen);
            resultMP = InfoManager.instance.GetGrowUpMP(this.level, mp);
            resultMPR = InfoManager.instance.GetGrowUpMPRecovery(this.level, mpRegen);
            resultAspd = InfoManager.instance.GetGrowUpAttackSpeed(this.level, attackSpeed);
            resultSPD = InfoManager.instance.GetGrowUpSpeed(this.level, speed);
        }

        totalATK = extraStat.extraAttack + resultAtk;
        totalASPD = extraStat.extraAttackSpeed + resultAspd;
        totalDEF = extraStat.extraDefense + resultDef;
        totalHP = extraStat.extraHP + resultHP;
        totalMP = extraStat.extraMP + resultMP;
        totalHPR = extraStat.extraHPR + resultHPR;
        totalMPR = extraStat.extraMPR + resultMPR;
        totalSPD = extraStat.extraSpeed + resultSPD;
        totalCritRate = extraStat.extraCritRate + critRate;
        totalCritDmg =  baseCritDamage + extraStat.extraCritDmg + critDmg;
    }

}
