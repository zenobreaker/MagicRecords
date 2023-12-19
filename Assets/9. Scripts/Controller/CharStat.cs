using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class CharStat
{
    public float baseCritDamage = 1.50f;

    public static readonly int MAX_CHAIN_POINT = 100;

    public MonsterGrade myGrade;
    public int level = 1;
    // 최종적으로 보여주는 스탯 
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
    public int totalCP;

    // 기본적으로 가지는 스탯 
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

    public float passiveAdditionalDamageRate = 0; //  패시브로 추가되는 데미지
    public float passiveAdditionalLostHealthRate = 0; // 패시브로 추가되는 잃은 체력 비례 데미지
    public float passiveAdditionalMaxHealthRate = 0; // 패시브로 추가되는 최대 체력 비례 데미지 
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
        myGrade = _target.myGrade;

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

    // 최대 경험치 계산
    public void CalcMaxExp(int level = 1)
    {
        this.maxExp = Mathf.FloorToInt(100 * Mathf.Pow(level, 1.5f));
    }

    //  
    public void GrowUp(int _exp)
    {
        this.exp += _exp; 
        while(this.exp >= this.maxExp)
        {
            this.level += level + 1;

            exp -= maxExp;
            CalcMaxExp(level);

            ApplyOption(); 
        }
    }
    
    public void ApplyEquipOption(ItemAbility itemAbility, bool isEquip)
    {
        if (extraStat == null)
            return;
   
        extraStat.ApplyOptionEquipStat(itemAbility, isEquip);
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
            + (int)extraStat.extraAttack;
        totalDEF = (int)Mathf.Round(resultDef * 1.0f * extraStat.increaseDefenseRate) 
            + (int)extraStat.extraDefense;
        totalHP = (int)Mathf.Round(resultHP * 1.0f * extraStat.increaseHPRate)
           + (int)extraStat.extraHP;
        totalHPR = (int)Mathf.Round(resultHPR * 1.0f * extraStat.increaseHPRegenRate)
           + (int)extraStat.extraMPR;
        totalMP = (int)Mathf.Round(resultMP * 1.0f * extraStat.increaseMPRate)
           + (int)extraStat.extraMP;
        totalMPR = (int)Mathf.Round(resultMPR * 1.0f * extraStat.increaseMPRegenRate)
           + (int)extraStat.extraMPR;
        totalSPD = (int)Mathf.Round(resultSPD * 1.0f * extraStat.increaseSpeedRate)
           + (int)extraStat.extraSpeed;

        totalCP = MAX_CHAIN_POINT;

        // 특수한 스탯은 계산식이 다르다
        totalASPD = (1.0f + resultAspd + extraStat.extraAttackSpeed) * 
                extraStat.increaseAttackSpeedRate ;
        totalCritRate = (extraStat.extraCritRate + critRate ) * 
            extraStat.increaseCritRate;
        totalCritDmg =  (baseCritDamage + extraStat.extraCritDmg + critDmg) *
            extraStat.increaseCritDmg ;

        // 버프 수치 
        totalATK = (int)(totalATK + (totalATK * extraStat.buffAttackRate));
        totalDEF = (int)(totalDEF + (totalDEF * extraStat.buffDefenseRate));
        totalHP = (int)(totalHP + (totalHP * extraStat.buffHPRate));
        totalHPR = (int)(totalHPR + (totalHPR * extraStat.buffHPRegenRate));
        totalMP = (int)(totalMP + (totalMP * extraStat.buffMPRate));
        totalMPR = (int)(totalMPR + (totalMPR * extraStat.buffMPRegenRate));
        totalSPD = (int)(totalSPD + (totalSPD * extraStat.buffSpeedRate));
        totalASPD += (totalASPD * extraStat.buffAttackSpeedRate);
        totalCritRate += totalCritRate * extraStat.buffCritRate;
        totalCritDmg += totalCritDmg * extraStat.buffCritDmg;

    }

    public CharStat  Clone()
    {
        return new CharStat(this);
    }
}

