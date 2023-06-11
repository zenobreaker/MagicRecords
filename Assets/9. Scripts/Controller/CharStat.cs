using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharStat
{
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


    // 기본적으로 갖고 있는 스탯 
    public int attack;
    public int defense;
    public float attackSpeed;
    public int hp;
    public int mp;
    public int hpRecovery;
    public int mpRecovery;
    public int speed;

    public int exp;
    public int maxExp;

    public ExtraStat extraStat = new ExtraStat();

    public CharStat(CharStat _target)
    {
        Debug.Log("CharStat Deep Copy");
        attack = _target.attack;
        defense = _target.defense;
        attackSpeed = _target.attackSpeed;
        hp = _target.hp;
        mp = _target.mp;
        hpRecovery = _target.hpRecovery;
        mpRecovery = _target.mpRecovery;
        speed = _target.speed;

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
            resultHPR = InfoManager.instance.GetGrowUpHPRecovery(this.level, hpRecovery);
            resultMP = InfoManager.instance.GetGrowUpMP(this.level, mp);
            resultMPR = InfoManager.instance.GetGrowUpMPRecovery(this.level, mpRecovery);
            resultAspd = InfoManager.instance.GetGrowUpAttackSpeed(this.level, attackSpeed);
            resultSPD = InfoManager.instance.GetGrowUpSpeed(this.level, speed);
        }

        totalATK = extraStat.extraAttack + resultAtk;
        totalASPD = extraStat.extraAttackSpeed + attackSpeed;
        totalDEF = extraStat.extraDefense + resultDef;
        totalHP = extraStat.extraHP + resultHP;
        totalMP = extraStat.extraMP + resultMP;
        totalHPR = extraStat.extraHPR + resultHPR;
        totalMPR = extraStat.extraMPR + resultMPR;
        totalSPD = extraStat.extraSpeed + resultSPD;
    }

}
