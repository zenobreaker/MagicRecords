using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharStat
{
    public float baseCritDamage = 1.50f;

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

    public CharStat()
    {
        attack = 0;
        defense = 0;
        attackSpeed = 0;
        hp = 0;
        mp = 0;
        hpRegen = 0;
        mpRegen = 0;
        speed = 0;
        critRate = 0;
        critDmg = 0;
        exp = 0;
        maxExp = 0;
        giveChanceExp = 0; 
    }

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

    // 캐릭터 경험치 계산처리
    public void CalcMaxExp(int level = 1)
    {
        this.maxExp = Mathf.FloorToInt(100 * Mathf.Pow(level, 1.5f));
    }

    //  캐릭터 성장 
    public void GrowUp(int _exp)
    {
        this.exp += _exp; 
        while(this.exp >= this.maxExp)
        {
            // 각 스탯별 능력치 증가 todo 계산식을 받아서 처리해보도록 하장
            this.level += level + 1;

            exp -= maxExp;
            CalcMaxExp(level);

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
        
        totalATK = (int)Mathf.Round(resultAtk * 1.0f * extraStat.increaseAttackRate) 
            + extraStat.extraAttack;
        totalDEF = (int)Mathf.Round(resultDef * 1.0f * extraStat.increaseDefenseRate) 
            + extraStat.extraDefense;
        totalHP = (int)Mathf.Round(resultHP * 1.0f * extraStat.increaseHPRate)
           + extraStat.extraHP;
        totalHPR = (int)Mathf.Round(resultHPR * 1.0f * extraStat.increaseHPRegenRate)
           + extraStat.extraMPR;
        totalMP = (int)Mathf.Round(resultMP * 1.0f * extraStat.increaseMPRate)
           + extraStat.extraMP;
        totalMPR = (int)Mathf.Round(resultMPR * 1.0f * extraStat.increaseMPRegenRate)
           + extraStat.extraMPR;
        totalSPD = (int)Mathf.Round(resultSPD * 1.0f * extraStat.increaseSpeedRate)
           + extraStat.extraSpeed;
       
        // 성격이 다른 스탯 
        totalASPD = extraStat.extraAttackSpeed + resultAspd;
        totalCritRate = extraStat.extraCritRate + critRate;
        totalCritDmg =  baseCritDamage + extraStat.extraCritDmg + critDmg;
    }

}
