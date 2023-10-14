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
    public Sprite sprite; 
    public bool isRunning = false;
    public int buffCount = 0;   // 버프 카운트

    public bool isRefresh = true;  // 갱신 가능한 버프인지 체크

    public bool buffCallFlag = false;
    public float buffCallTime = 0;     // 버프 기능을 실행하는 주기

    public void Init(BuffType buffType, string name, SpecialOption option)
    {
        this.buffType = buffType;
        this.buffName = name;
        specialOption = option;
        
        if (specialOption != null)
        {
            specialOption.SetCoolTime();
        }
    }

    public void Activation(WheelerController wheeler)
    {
        if (isRunning == true) return; 
        isRunning = true;

        if (specialOption != null && wheeler != null 
            && wheeler.MyPlayer != null)
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
                        wheeler.MyPlayer.MyStat.extraStat.ApplyOptionExtraStat(
                            specialOption.abilityType,
                             specialOption.value, true);
                        wheeler.MyPlayer.MyStat.ApplyOption();
                        break;
                    case AbilityType.HOLD:
                        break;
                    case AbilityType.SLOW:
                        break;
                    case AbilityType.STURN:
                        break;
                    case AbilityType.ICE:
                        break;

                        // 특수 - 증오의 저주 
                    case AbilityType.CURSE_HATED:
                        // 매 초마다 시전자의 공격력 비만큼 피해를 입는다. 

                        break; 

                }
            }
            else
            {
                wheeler.MyPlayer.MyStat.extraStat.ApplyOptionExtraStat(specialOption.abilityType,
                    specialOption.value, specialOption.isPercentage);
            }

        }
    }

    public void Excute(WheelerController wheeler)
    {
        if (wheeler == null) return;

        if (specialOption != null && wheeler.MyPlayer != null)
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
                        break;
                    case AbilityType.HOLD:
                        break;
                    case AbilityType.SLOW:
                        break;
                    case AbilityType.STURN:
                        break;
                    case AbilityType.ICE:
                        break;

                    // 특수 - 증오의 저주 
                    case AbilityType.CURSE_HATED:
                        // 매 초마다 시전자의 공격력 비만큼 피해를 입는다. 
                        // deald
                        wheeler.DotDamage((int)specialOption.value);
                        break;

                }
            }
        }

    }

    public void Deactivation(WheelerController wheeler)
    {
        isRunning = false; 

        if (wheeler == null || wheeler.MyPlayer == null) return;



        wheeler.MyPlayer.MyStat.extraStat.ApplyOptionExtraStat(specialOption.abilityType,
            -specialOption.value, specialOption.isPercentage);
        wheeler.MyPlayer.MyStat.ApplyOption();
    }

}
