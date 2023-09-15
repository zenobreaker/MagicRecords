using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuffType
{ 
    BUFF =1,
    DEBUFF=2,
}
public enum Buff
{
    NONE = 0,
    INCREASE_ATTACK,
    INCREASE_DEFENSE,
    INCREASE_SPEED,
    INCREASE_ATTACK_SPEED,
}

public enum Debuff
{
    NONE = 0,
    BREAK_AROMR,        // 방어구 파괴 (방어력 감소)
    BREAK_WEAPON,       // 무기 파괴 (공격력 감소) 
    DOWN_ATTACK_SPEED,  // 공격속도 감소

    BLEED,              // 출혈  - 지속 데미지 + 공격력 감소 
    BURN,               // 화상 - 지속 데미지 + 방어력 감소
    CURSE,              // 저주 (모든 스테이터스 감소)
    HOLD,               // 속박 - 이동불가 
    SLOW,               // 둔화 - 공격속도 / 이동속도 감소
    STURN,              // 기절 - 공격/이동 불가 
    ICE,                // 빙결 - 기절과 같음
}


public class BuffDebuff 
{
    public BuffType buffType;
    public string buffName;
    public SpecialOption specialOption;
    public Image icon;
    public bool isRunning = false; 
    WaitForSeconds seconds = new WaitForSeconds(0.1f);

    public void Init(BuffType buffType, string name, SpecialOption option)
    {
        this.buffType = buffType;
        this.buffName = name;
        specialOption = option;
        if(specialOption != null)
        {
            specialOption.SetCoolTime();
        }
    }

    public void Activation(Character character)
    {
        if (isRunning == true) return; 
        isRunning = true;

        if (specialOption != null && character != null)
        {
            // 버프 상태에 따른 효과 적용
            if (specialOption.optionType == OptionType.DEBUFF)
            {
                switch (specialOption.abilityType)
                {
                    case AbilityType.BREAK_AROMR:
                        break;
                    case AbilityType.BREAK_WEAPON:
                        break;
                    case AbilityType.DOWN_ATTACK_SPEED:
                        break;
                    case AbilityType.BLEED:
                        break;
                    case AbilityType.BURN:
                        break;
                    case AbilityType.CURSE:
                        // 공격력과 방어력을 10%씩 감소 시킨다. 
                        character.MyStat.extraStat.ApplyOptionExtraStat(
                            specialOption.abilityType,
                             specialOption.value, true);
                        character.MyStat.ApplyOption();
                        break;
                    case AbilityType.HOLD:
                        break;
                    case AbilityType.SLOW:
                        break;
                    case AbilityType.STURN:
                        break;
                    case AbilityType.ICE:
                        break;

                }
            }
            else
            {
                character.MyStat.extraStat.ApplyOptionExtraStat(specialOption.abilityType,
                    specialOption.value, specialOption.isPercentage);
            }

        }

        BuffManager.instance.StartBuffTimer(character, this);
    }

    public void Deactivation(Character character)
    {
        isRunning = false; 

        if (character == null) return;


        character.MyStat.extraStat.ApplyOptionExtraStat(specialOption.abilityType,
            -specialOption.value, specialOption.isPercentage);
        character.MyStat.ApplyOption();
    }

}
