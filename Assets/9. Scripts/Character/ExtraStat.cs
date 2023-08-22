using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExtraStat 
{
    public int extraAttack;
    public int extraDefense;
    public int extraSpeed;
    public int extraHP;
    public int extraMP;
    public int extraHPR;
    public int extraMPR;
    public float extraAttackSpeed;
    public float extraCritRate;
    public float extraCritDmg;


    // 퍼센트 증가 비율 총합 수치를 담는 변수들 
    public float increaseAttackRate;         // 공격력 증가률 
    public float increaseDefenseRate;        // 방어력 증가율
    public float increaseSpeedRate;          // 이동속도 증가율
    public float increaseHPRate;             // 체력 증가율
    public float increaseHPRegenRate;        // 초당 체력 회복 증가율
    public float increaseMPRate;         // 마나 증가율
    public float increaseMPRegenRate;         // 초당 마나 회복 증가율 
    
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

        increaseAttackRate = 1;
        increaseDefenseRate = 1;        // 방어력 증가율
        increaseSpeedRate = 1;          // 이동속도 증가율
        increaseHPRate = 1;             // 체력 증가율
        increaseHPRegenRate = 1;        // 초당 체력 회복 증가율
        increaseMPRate = 1;         // 마나 증가율
        increaseMPRegenRate = 1;         // 초당 마나 회복 증가율 
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

        increaseAttackRate = 1;
        increaseDefenseRate = 1;        // 방어력 증가율
        increaseSpeedRate = 1;          // 이동속도 증가율
        increaseHPRate = 1;             // 체력 증가율
        increaseHPRegenRate = 1;        // 초당 체력 회복 증가율
        increaseMPRate = 1;         // 마나 증가율
        increaseMPRegenRate = 1;         // 초당 마나 회복 증가율 
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


    public void CalcOptionIncreaseStat(CharStat stat, AbilityType abilityType, float value, bool isPercentage = false)
    {

    }

    // 나는 그렇게 살다간.. 망할거야..
    // 증가율 수치를 계산하는 함수 
    // 여긴 고정된 수치 값을 총합시키는 함수
    public void CalcExtraStat(AbilityType abilityType, float value)
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


    // 여기선 백분율 값을 전부 더하는 기능을 한다. 
    public void CalcIncreaseRateStat(AbilityType abilityType, float value)
    {
        switch (abilityType)
        {
            case AbilityType.ATK:
                increaseAttackRate += value;
                break;
            case AbilityType.DEF:
                increaseDefenseRate += value;
                break;
            case AbilityType.ASPD:
                break;
            case AbilityType.SPD:
                increaseSpeedRate += value;
                break;
            case AbilityType.HP:
                increaseHPRate += value;
                break;
            case AbilityType.HPR:
                increaseHPRegenRate += value;
                break;
            case AbilityType.MP:
                increaseMPRate += value;
                break;
            case AbilityType.MPR:
                increaseMPRegenRate += value;
                break;
            case AbilityType.CRITRATE:
                // 크리티컬확률과 크리티컬 데미지 증가는 이미 별도로 계산처리함
                break;
            case AbilityType.CRITDMG:

                break;
        }
    }

    public void ApplyOptionExtraStat(AbilityType abilityType, float value, bool isPercent = false)
    {
        if (abilityType == AbilityType.NONE) return;

        // 백분율이므로 
        if(isPercent == true)
        {
            CalcIncreaseRateStat(abilityType, value);
        }
        else
        {
            CalcExtraStat(abilityType, value);
        }
    }


    public void ApplyOptionExtraStat(ItemAbility itemAbility, bool isSubbed = false)
    {
        float value = itemAbility.power;
        if(isSubbed == true)
        {
            value *= -1; 
        }

        ApplyOptionExtraStat(itemAbility.abilityType, value, itemAbility.isPercent);
    }

}
