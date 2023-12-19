using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExtraStat
{
	// 장비처럼 고정값이 증가하는 스탯
	public float extraAttack;
	public float extraDefense;
	public float extraSpeed;
	public float extraHP;
	public float extraMP;
	public float extraHPR;
	public float extraMPR;
	public float extraAttackSpeed;
	public float extraCritRate;
	public float extraCritDmg;

	// 퍼센트 증가 비율 총합 수치를 담는 변수들 
	public float increaseAttackRate;         // 공격력 증가률 
	public float increaseDefenseRate;        // 방어력 증가율
	public float increaseAttackSpeedRate;   // 공격속도 증가율
	public float increaseSpeedRate;          // 이동속도 증가율
	public float increaseHPRate;             // 체력 증가율
	public float increaseHPRegenRate;        // 초당 체력 회복 증가율
	public float increaseMPRate;         // 마나 증가율
	public float increaseMPRegenRate;         // 초당 마나 회복 증가율 
	public float increaseCritRate;         // 치명 확률
	public float increaseCritDmg;         // 치명타 피해

	// 버프에 의한 비율 총합 수치를 담는 변수들 
	public float buffAttackRate;         // 공격력 증가률 
	public float buffDefenseRate;        // 방어력 증가율
	public float buffAttackSpeedRate;   // 공격속도 증가율
	public float buffSpeedRate;          // 이동속도 증가율
	public float buffHPRate;             // 체력 증가율
	public float buffHPRegenRate;        // 초당 체력 회복 증가율
	public float buffMPRate;         // 마나 증가율
	public float buffMPRegenRate;         // 초당 마나 회복 증가율 
	public float buffCritRate;         // 치명 확률
	public float buffCritDmg;         // 치명타 피해


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
		extraCritRate = 0;
		extraCritDmg = 0;

		increaseAttackRate = 1;
		increaseDefenseRate = 1;        // 방어력 증가율
		increaseAttackSpeedRate = 1;    // 공격속도 증가율
		increaseSpeedRate = 1;          // 이동속도 증가율
		increaseHPRate = 1;             // 체력 증가율
		increaseHPRegenRate = 1;        // 초당 체력 회복 증가율
		increaseMPRate = 1;         // 마나 증가율
		increaseMPRegenRate = 1;         // 초당 마나 회복 증가율
		increaseCritRate = 1;         // 치명 확률
		increaseCritDmg = 1;         // 치명타 피해

		// 버프에 의한 비율 총합 수치를 담는 변수들 
		buffAttackRate = 0;         // 공격력 증가률 
		buffDefenseRate = 0;        // 방어력 증가율
		buffAttackSpeedRate = 0;    // 공격속도 증가율
		buffSpeedRate = 0;          // 이동속도 증가율
		buffHPRate = 0;             // 체력 증가율
		buffHPRegenRate = 0;        // 초당 체력 회복 증가율
		buffMPRate = 0;         // 마나 증가율
		buffMPRegenRate = 0;         // 초당 마나 회복 증가율 
		buffCritRate = 0;         // 치명 확률
		buffCritDmg = 0;         // 치명타 피해
	}

	public ExtraStat Clone()
	{
		ExtraStat stat = new ExtraStat();
		stat.extraAttack = extraAttack;
		stat.extraDefense = extraDefense;
		stat.extraAttackSpeed = extraAttackSpeed;
		stat.extraSpeed = extraSpeed;
		stat.extraHP = extraHP;
		stat.extraHPR = extraHPR;
		stat.extraMP = extraMP;
		stat.extraMPR = extraMPR;
		stat.extraCritRate = extraCritRate;
		stat.extraCritDmg = extraCritDmg;

		stat.increaseAttackRate = increaseAttackRate;
		stat.increaseDefenseRate = increaseDefenseRate;
		stat.increaseAttackSpeedRate = increaseAttackSpeedRate;
		stat.increaseSpeedRate = increaseSpeedRate;
		stat.increaseHPRate = increaseHPRate;
		stat.increaseHPRegenRate = increaseHPRegenRate;
		stat.increaseMPRate = increaseMPRate;
		stat.increaseMPRegenRate = increaseMPRegenRate;
		stat.increaseCritRate = increaseCritRate;
		stat.increaseCritDmg = increaseCritDmg;

		stat.buffAttackRate = buffAttackRate;
		stat.buffDefenseRate = buffDefenseRate;
		stat.buffAttackSpeedRate = buffAttackSpeedRate;
		stat.buffSpeedRate = buffSpeedRate;
		stat.buffHPRate = buffHPRate;
		stat.buffHPRegenRate = buffHPRegenRate;
		stat.buffMPRate = buffMPRate;
		stat.buffMPRegenRate = buffMPRegenRate;
		stat.buffCritRate = buffCritRate;
		stat.buffCritDmg = buffCritDmg;

		return stat;
	}

	public void IncreaseHP(float _value)
	{
		extraHP += _value;
		if (extraHP < 0)
		{
			extraHP = 0;
		}
	}
	public void IncreaseMP(float _value)
	{
		extraMP += _value;
		if (extraMP < 0)
		{
			extraMP = 0;
		}
	}

	public void IncreaseAtk(float _value)
	{
		extraAttack += _value;
		if (extraAttack < 0)
		{
			extraAttack = 0;
		}
	}

	public void IncreaseAtkSpd(float _value)
	{
		extraAttackSpeed += _value;
	}

	public void IncreaseDef(float _value)
	{
		extraDefense += _value;
		if (extraDefense < 0)
		{
			extraDefense = 0;
		}
	}
	public void IncreaseSpd(float _value)
	{
		extraSpeed += _value;
	}

	public void IncreaseHPR(float _value)
	{
		extraHPR += _value;
		if (extraHPR < 0)
		{
			extraHPR = 0;
		}
	}

	public void IncreaseMPR(float _value)
	{
		extraSpeed += _value;
		if (extraSpeed < 0)
		{
			extraSpeed = 0;
		}
	}

	public void IncreaseCritRate(float _value)
	{
		extraCritRate += _value;
	}

	public void IncreaseCritDmg(float _value)
	{
		extraCritDmg += _value;
		if (extraCritDmg < 0)
		{
			extraCritDmg = 0;
		}
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




	// 증가율 수치를 계산하는 함수 
	// 여긴 고정된 수치 값을 총합시키는 함수
	public void CalcExtraStat(AbilityType abilityType, float value)
	{
		if (abilityType == AbilityType.NONE) return;

		switch (abilityType)
		{
			case AbilityType.ATK:
				IncreaseAtk(value);
				break;
			case AbilityType.DEF:
				IncreaseDef(value);
				break;
			case AbilityType.ASPD:
				IncreaseAtkSpd(value);
				break;
			case AbilityType.SPD:
				IncreaseSpd(value);
				break;
			case AbilityType.HP:
				IncreaseHP(value);
				break;
			case AbilityType.HPR:
				IncreaseHPR(value);
				break;
			case AbilityType.MP:
				IncreaseMP(value);
				break;
			case AbilityType.MPR:
				IncreaseMPR(value);
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
				increaseAttackSpeedRate += value;
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
				increaseCritRate += value;
				break;
			case AbilityType.CRITDMG:
				increaseCritDmg += value;
				break;
		}
	}


	// 버프값
	public void CalcbuffStat(AbilityType abilityType, float value)
	{
		switch (abilityType)
		{
			case AbilityType.ATK:
				buffAttackRate += value;
				break;
			case AbilityType.DEF:
				buffDefenseRate += value;
				break;
			case AbilityType.ASPD:
				buffAttackSpeedRate += value;
				break;
			case AbilityType.SPD:
				buffSpeedRate += value;
				break;
			case AbilityType.HP:
				buffHPRate += value;
				break;
			case AbilityType.HPR:
				buffHPRegenRate += value;
				break;
			case AbilityType.MP:
				buffMPRate += value;
				break;
			case AbilityType.MPR:
				buffMPRegenRate += value;
				break;
			case AbilityType.CRITRATE:
				buffCritRate += value;
				break;
			case AbilityType.CRITDMG:
				buffCritRate += value;
				break;

			// 저주가 걸리면 공,방 수치가  10%씩 감소한다.
			case AbilityType.CURSE:
				buffAttackRate -= value;
				buffDefenseRate -= value;
				break;
		}
	}



	public void ApplyOptionExtraStat(AbilityType abilityType, float value, bool isPercent = false)
	{
		if (abilityType == AbilityType.NONE) return;

		// 백분율이므로 
		if (isPercent == true)
		{
			CalcIncreaseRateStat(abilityType, value);
		}
		else
		{
			CalcExtraStat(abilityType, value);
		}
	}


	public void ApplyBuffOptionStat(AbilityType abilityType, float value)
	{
		CalcbuffStat(abilityType, value);
	}

	public void ApplyOptionExtraStat(ItemAbility itemAbility, bool isSubbed = false)
	{
		float value = itemAbility.power;
		if (isSubbed == true)
		{
			value *= -1;
		}

		ApplyOptionExtraStat(itemAbility.abilityType, value, itemAbility.isPercent);
	}

	// 장비 능력치 적용 
	public void ApplyOptionEquipStat(ItemAbility itemAbility, bool isEquip)
	{
        float value = itemAbility.power;
		// 장비 수치는 int형이므로 percent 변수가 체크되면 소수점화시킨다.
		if (itemAbility.isPercent == true)
			value = value * 0.01f;

		if (isEquip == false)
			value = value * -1;
	
        ApplyOptionExtraStat(itemAbility.abilityType, value, itemAbility.isPercent);
    }
}
