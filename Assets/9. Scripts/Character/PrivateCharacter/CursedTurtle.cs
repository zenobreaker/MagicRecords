using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.TextCore.Text;


// 기본 캐릭터 : 남생이
// 금기 -  저주 사용자
public class CursedTurtle : PassiveClass
{

    // 여기선 패시브 스킬들을 정의한다. 

    public override void ApplyPassiveSkillEffects(Character target)
    {
        base.ApplyPassiveSkillEffects(target);

        foreach (var skill in equippedPassiveSkills)
        {
            if (skill == null || skill.isUnlocked == false) continue; 

            switch(skill.keycode)
            {
                // 1. 금지된 마술 
                case "forbiddenMagic":
                    ForbiddenMagic(skill, target);
                    break;
                case "injection-Elemental":
                    break;
                // 마탄 개조
                case "renovatedBullet":
                    RenovateBullet(skill);
                    break;

                // 금기 - 모멸하는 비탄 
                case "forbidden-Grief": 
                    // character는 적 
                    ForbiddenGrief(skill, target);
                    break;
                // 금기 - 가증스러운 증오
                case "forbidden-Hatred":
                    ForbiddenHatred(skill, target);
                    break;
                // 금기 - 파멸하는 분노
                case "forbidden-Anger":
                    ForbiddenAnger(skill, target);
                    break;
                // 저편으로부터의 저주
                case "curseBeyond": break;
            }

        }

      
    }


    // 금지된 마술 - 저주
    private void ForbiddenMagic(Skill skill, Character target)
    {
        // 피해를 입힌 적에게 저주 상태 부여 10()초
        if(target != null && skill != null)
        {
            // 저주버프를 만든다.
            foreach (var option in skill.bonusSpecialOptionList)
            {
                if(option.optionType == OptionType.BUFF)
                {
                    // 이 스킬은 버프는 주지않으니 넘긴다.
                    continue;
                }

                BuffDebuff buffDebuff = new BuffDebuff();
                buffDebuff.buffName = "Curse";
                buffDebuff.Init(BuffType.DEBUFF, option.effectName, option);
                target.ApplyBuffDebuff(buffDebuff);
            }
                    
        }
        // 공격력과 치명타 상승 
        else if(skill != null && charStat != null)
        {
            // 나 자신을 사랑하라.. 
            // 이건 버프가 아닌 스탯 영구 지속이니 장비처럼 강화한다.
            // 이런식으로 구현한 이유는 외부 파일을 통해 수정을 용이하게 해보기 위함?이다..
            foreach(var option in skill.bonusSpecialOptionList)
            {
                if(option == null || option.optionType == OptionType.DEBUFF)
                {
                    continue; 
                }

                // 보아라 이 힘을..!
                Debug.Log("패시브 효과 : " + option.abilityType + " " + option.value);
                charStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                    option.isPercentage);
            }

            charStat.ApplyOption();
        }
    }


    // 마탄 개조 
    public void RenovateBullet(Skill skill)
    {
        if (skill == null || charStat == null) return;
        // 일반 공격 피해가 범위 피해로 변경 
        // todo 미구현 

        // 공격력 증가
        foreach (var option in skill.bonusSpecialOptionList)
        {
            if (option == null || option.optionType == OptionType.DEBUFF)
            {
                continue;
            }

            // 보아라 이 힘을..!
            Debug.Log("패시브 효과 : " + option.abilityType + " " + option.value);
            charStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                option.isPercentage);
        }

        charStat.ApplyOption();

    }

    // 금기 - 모멸하는 비탄
    public void ForbiddenGrief(Skill skill, Character target)
    {
        if (skill == null || target == null || charStat == null) return;

        // 상태이상을 하나 이상 갖고 있는 대상에게 잃은 체력의 비례하는 추가 데미지를 입힌다.
        // 상태이상 카운트
        int debuffCount = 0; 
        foreach(var buff in target.buffDebuffs)
        {
            if (buff == null || buff.specialOption == null) continue;

            if(buff.specialOption.optionType == OptionType.DEBUFF)
            {
                debuffCount++;
                break; 
            }
        }

        if (debuffCount <= 0) return;

        // 하나 이상 있으니 계산

        // 대상의 잃은 체력 비례 데미지... 
        float lostHealthPercentage = (target.MyMaxHP - target.MyCurrentHP) / (float)target.MyMaxHP;
        target.MyStat.passiveAdditionalLostHealthRate = lostHealthPercentage;

    }

    // 금기 - 가증스러운 증오
    public void ForbiddenHatred(Skill skill, Character target)
    {
        if (skill == null || target == null || charStat == null) return;

        // 지속적으로 피해를 입는 "증오의 저주" 디버프 부여 
        if (target != null && skill != null)
        {
            // 저주버프를 만든다.
            foreach (var option in skill.bonusSpecialOptionList)
            {
                if (option.optionType == OptionType.BUFF)
                {
                    // 이 스킬은 버프는 주지않으니 넘긴다.
                    continue;
                }

                BuffDebuff buffDebuff = new BuffDebuff();
                buffDebuff.buffName = "HatredCurse";
                buffDebuff.Init(BuffType.DEBUFF, option.effectName, option);
                // 옵션을 선택해서 기능을 할당
                if(buffDebuff.specialOption != null)
                {
                    float rate = 0.03f;
                    buffDebuff.buffCallFlag = true;
                    buffDebuff.buffCallTime = 1.0f;
                    buffDebuff.specialOption.duration = 3;
                    buffDebuff.specialOption.value = 1.0f + Mathf.Round(charStat.totalATK * rate);
                }
                target.ApplyBuffDebuff(buffDebuff);
            }
        }
    }

    // 금기 - 파멸하는 분노
    public void ForbiddenAnger(Skill skill, Character target)
    {
        if (skill == null || target == null || charStat == null) return;

        // 1. 치명타 피해 증가 
        if (skill != null)
        {
            // 나 자신을 사랑하라.. 
            // 이건 버프가 아닌 스탯 영구 지속이니 장비처럼 강화한다.
            // 이런식으로 구현한 이유는 외부 파일을 통해 수정을 용이하게 해보기 위함?이다..
            foreach (var option in skill.bonusSpecialOptionList)
            {
                if (option == null || option.optionType == OptionType.DEBUFF)
                {
                    continue;
                }

                // 보아라 이 힘을..!
                Debug.Log("패시브 효과 : " + option.abilityType + " " + option.value);
                charStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                    option.isPercentage);
            }

            charStat.ApplyOption();
        }
        // 2. 대상이 가지고 있는 상태이상의 개수 만큼 추가 피해
        // 1개 : 5 % 2개 10 % 3개 15% 
        if (target != null)
        {
            int debuffCount = 0;
            foreach (var buff in target.buffDebuffs)
            {
                if (buff == null || buff.specialOption == null) continue;

                if (buff.specialOption.optionType == OptionType.DEBUFF)
                {
                    debuffCount++;
                    break;
                }
            }

            // 1개 이상 2개 미만
            if(debuffCount >= 1 && debuffCount < 2)
            {
                target.MyStat.passiveAdditionalDamageRate = 0.05f;
            }
            // 2개 이상 3개 미만
            else if (debuffCount >= 1 && debuffCount < 2)
            {
                target.MyStat.passiveAdditionalDamageRate = 0.1f;
            }
            // 3개 이상
            else if (debuffCount >= 3)
            {
                target.MyStat.passiveAdditionalDamageRate = 0.15f;
            }
        }
    }
}
