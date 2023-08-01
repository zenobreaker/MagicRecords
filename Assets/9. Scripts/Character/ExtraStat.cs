using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExtraStat 
{
    public int extraAttack;
    public float extraAttackSpeed;
    public int extraDefense;
    public int extraSpeed;
    public int extraHP;
    public int extraMP;
    public int extraHPR;
    public int extraMPR;
    public float extraCritRate;
    public float extraCritDmg;

    public ExtraStat()
    {
        extraAttack = 0;
        extraAttackSpeed = 0;
        extraDefense = 0;
        extraSpeed = 0;
        extraHP = 0;
        extraMP = 0;
        extraHPR = 0;
        extraMPR = 0;
        extraCritRate = 0;
        extraCritDmg = 0; 
    }

    public ExtraStat(int _atk, float _atkspd, int _def, int _spd, int _hp, int _hpr, int _mp, int _mpr)
    {
        extraAttack  = _atk;
        extraAttackSpeed = _atkspd;
        extraDefense = _def;
        extraSpeed = _spd;
        extraHP = _hp;
        extraMP = _mp;
        extraHPR = _hpr;
        extraMPR = _mpr;
    }

    public void IncreaseHP(int _value)
    {
        extraHP += _value;
    }
    public void IncreaseMP(int _value)
    {
        extraMP += _value;
    }

    public void IncreaseAtk(int _value)
    {
        extraAttack += _value;
    }

    public void IncreaseAtkSpd(int _value)
    {
        extraAttackSpeed += _value;
    }

    public void IncreaseDef(int _value)
    {
        extraDefense += _value;
    }
    public void IncreaseSpd(int _value)
    {
        extraSpeed += _value;
    }

    public void IncreaseHPR(int _value)
    {
        extraHPR += _value;
    }

    public void IncreaseMPR(int _value)
    {
        extraSpeed += _value;
    }

    public void IncreaseCritRate(float _value)
    {
        extraCritRate += _value; 
    }

    public void IncreaseCritDmg(float _value)
    {
        extraCritDmg += _value;
    }

    public void ClearStat()
    {
        extraAttack = 0;
        extraDefense = 0;
        extraAttackSpeed = 0;
        extraSpeed = 0;
        extraHP = 0;
        extraHPR = 0;
        extraMP = 0;
        extraMPR = 0;
        extraCritRate = 0;
        extraCritDmg = 0;
    }


    public void SetStatus(ExtraStat extraStat)
    {
        extraAttack = extraStat.extraAttack;
        extraAttackSpeed = extraStat.extraAttackSpeed;
        extraDefense = extraStat.extraDefense;
        extraSpeed = extraStat.extraSpeed;
        extraHP = extraStat.extraHP;
        extraMP = extraStat.extraMP;
        extraHPR = extraStat.extraHPR;
        extraMPR = extraStat.extraMPR;
    }

    public void AddedStatus(ExtraStat extraStat)
    {
        extraAttack += extraStat.extraAttack;
        extraAttackSpeed += extraStat.extraAttackSpeed;
        extraDefense += extraStat.extraDefense;
        extraSpeed += extraStat.extraSpeed;
        extraHP += extraStat.extraHP;
        extraMP += extraStat.extraMP;
        extraHPR += extraStat.extraHPR;
        extraMPR += extraStat.extraMPR;
        extraCritRate += extraStat.extraCritRate;
        extraCritDmg += extraStat.extraCritDmg;
    }

    public void SubbedStatus(ExtraStat extraStat)
    {
        extraAttack -= extraStat.extraAttack;
        extraAttackSpeed -= extraStat.extraAttackSpeed;
        extraDefense -= extraStat.extraDefense;
        extraSpeed -= extraStat.extraSpeed;
        extraHP -= extraStat.extraHP;
        extraMP -= extraStat.extraMP;
        extraHPR -= extraStat.extraHPR;
        extraMPR -= extraStat.extraMPR;
        extraCritRate -= extraStat.extraCritRate;
        extraCritDmg -= extraStat.extraCritDmg;
    }


    public void ApplyOptionExtraStat(AbilityType abilityType, float value)
    {
        if (abilityType == AbilityType.NONE) return;

        switch (abilityType)
        {
            case AbilityType.ATK:
                IncreaseAtk((int)value);
                break;
            case AbilityType.DEF:
                IncreaseDef((int)value);
                break;
            case AbilityType.ASPD:
                IncreaseAtkSpd((int)value);
                break;
            case AbilityType.SPD:
                IncreaseSpd((int)value);
                break;
            case AbilityType.HP:
                IncreaseHP((int)value);
                break;
            case AbilityType.HPR:
                IncreaseHPR((int)value);
                break;
            case AbilityType.MP:
                IncreaseMP((int)value);
                break;
            case AbilityType.MPR:
                IncreaseMPR((int)value);
                break;
            case AbilityType.CRITRATE:
                IncreaseCritRate(value); 
                break;
            case AbilityType.CRITDMG:
                IncreaseCritDmg(value);
                break;
        }
    }

}
