using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


// �⺻ ĳ���� : ������
// �ݱ� -  ���� �����
public class CursedTurtle : Character
{

    // ���⼱ �нú� ��ų���� �����Ѵ�. 

    public override void ApplyPassiveSkillEffects(Character character)
    {
        base.ApplyPassiveSkillEffects(character);

        foreach (var skill in equippedPassiveSkills)
        {
            if (skill == null || skill.isUnlocked == false) continue; 

            switch(skill.keycode)
            {
                // 1. ������ ���� 
                case "forbiddenMagic":
                    ForbiddenMagic(skill, character);
                    break;
                case "injection-Elemental":
                    break;
                // ��ź ����
                case "renovatedBullet":break;
                // �ݱ� - ����ϴ� ��ź 
                case "forbidden-Grief": break;
                // �ݱ� - ���������� ����
                case "forbidden-Hatred": break;
                // �ݱ� - �ĸ��ϴ� �г�
                case "forbidden-Anger": break;
                // �������κ����� ����
                case "curseBeyond": break;
            }

        }

      
    }


    // ������ ���� - ����
    private void ForbiddenMagic(Skill skill, Character character)
    {
        // ���ظ� ���� ������ ���� ���� �ο� 10()��
        if(character != null && skill != null)
        {
            // ���ֹ����� �����.
            foreach (var option in skill.bonusSpecialOptionList)
            {
                if(option.optionType == OptionType.BUFF)
                {
                    // �� ��ų�� ������ ���������� �ѱ��.
                    continue;
                }

                BuffDebuff buffDebuff = new BuffDebuff();
                buffDebuff.buffName = "Curse";
                buffDebuff.Init(BuffType.DEBUFF, option.effectName, option);
                character.ApplyBuffDebuff(buffDebuff);
            }
                    
        }
        // ���ݷ°� ġ��Ÿ ��� 
        else
        {

        }
    }



}
