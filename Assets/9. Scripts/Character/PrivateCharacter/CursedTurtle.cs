using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


// 기본 캐릭터 : 남생이
// 금기 -  저주 사용자
public class CursedTurtle : Character
{

    // 여기선 패시브 스킬들을 정의한다. 

    public override void ApplyPassiveSkillEffects(Character character)
    {
        base.ApplyPassiveSkillEffects(character);

        foreach (var skill in equippedPassiveSkills)
        {
            if (skill == null || skill.isUnlocked == false) continue; 

            switch(skill.keycode)
            {
                // 1. 금지된 마술 
                case "forbiddenMagic":
                    ForbiddenMagic(skill, character);
                    break;
                case "injection-Elemental":
                    break;
                // 마탄 개조
                case "renovatedBullet":break;
                // 금기 - 모멸하는 비탄 
                case "forbidden-Grief": break;
                // 금기 - 가증스러운 증오
                case "forbidden-Hatred": break;
                // 금기 - 파멸하는 분노
                case "forbidden-Anger": break;
                // 저편으로부터의 저주
                case "curseBeyond": break;
            }

        }

      
    }


    // 금지된 마술 - 저주
    private void ForbiddenMagic(Skill skill, Character character)
    {
        // 피해를 입힌 적에게 저주 상태 부여 10()초
        if(character != null && skill != null)
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
                character.ApplyBuffDebuff(buffDebuff);
            }
                    
        }
        // 공격력과 치명타 상승 
        else
        {

        }
    }



}
