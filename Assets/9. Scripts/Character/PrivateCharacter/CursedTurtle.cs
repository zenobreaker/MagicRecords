using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.TextCore.Text;


// �⺻ ĳ���� : ������
// �ݱ� -  ���� �����
public class CursedTurtle : PassiveClass
{

    // ���⼱ �нú� ��ų���� �����Ѵ�. 

    public override void ApplyPassiveSkillEffects(Character target)
    {
        base.ApplyPassiveSkillEffects(target);

        foreach (var skill in equippedPassiveSkills)
        {
            if (skill == null || skill.isUnlocked == false) continue; 

            switch(skill.keycode)
            {
                // 1. ������ ���� 
                case "forbiddenMagic":
                    ForbiddenMagic(skill, target);
                    break;
                case "injection-Elemental":
                    break;
                // ��ź ����
                case "renovatedBullet":
                    RenovateBullet(skill);
                    break;

                // �ݱ� - ����ϴ� ��ź 
                case "forbidden-Grief": 
                    // character�� �� 
                    ForbiddenGrief(skill, target);
                    break;
                // �ݱ� - ���������� ����
                case "forbidden-Hatred":
                    ForbiddenHatred(skill, target);
                    break;
                // �ݱ� - �ĸ��ϴ� �г�
                case "forbidden-Anger":
                    ForbiddenAnger(skill, target);
                    break;
                // �������κ����� ����
                case "curseBeyond": break;
            }

        }

      
    }


    // ������ ���� - ����
    private void ForbiddenMagic(Skill skill, Character target)
    {
        // ���ظ� ���� ������ ���� ���� �ο� 10()��
        if(target != null && skill != null)
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
                target.ApplyBuffDebuff(buffDebuff);
            }
                    
        }
        // ���ݷ°� ġ��Ÿ ��� 
        else if(skill != null && charStat != null)
        {
            // �� �ڽ��� ����϶�.. 
            // �̰� ������ �ƴ� ���� ���� �����̴� ���ó�� ��ȭ�Ѵ�.
            // �̷������� ������ ������ �ܺ� ������ ���� ������ �����ϰ� �غ��� ����?�̴�..
            foreach(var option in skill.bonusSpecialOptionList)
            {
                if(option == null || option.optionType == OptionType.DEBUFF)
                {
                    continue; 
                }

                // ���ƶ� �� ����..!
                Debug.Log("�нú� ȿ�� : " + option.abilityType + " " + option.value);
                charStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                    option.isPercentage);
            }

            charStat.ApplyOption();
        }
    }


    // ��ź ���� 
    public void RenovateBullet(Skill skill)
    {
        if (skill == null || charStat == null) return;
        // �Ϲ� ���� ���ذ� ���� ���ط� ���� 
        // todo �̱��� 

        // ���ݷ� ����
        foreach (var option in skill.bonusSpecialOptionList)
        {
            if (option == null || option.optionType == OptionType.DEBUFF)
            {
                continue;
            }

            // ���ƶ� �� ����..!
            Debug.Log("�нú� ȿ�� : " + option.abilityType + " " + option.value);
            charStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                option.isPercentage);
        }

        charStat.ApplyOption();

    }

    // �ݱ� - ����ϴ� ��ź
    public void ForbiddenGrief(Skill skill, Character target)
    {
        if (skill == null || target == null || charStat == null) return;

        // �����̻��� �ϳ� �̻� ���� �ִ� ��󿡰� ���� ü���� ����ϴ� �߰� �������� ������.
        // �����̻� ī��Ʈ
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

        // �ϳ� �̻� ������ ���

        // ����� ���� ü�� ��� ������... 
        float lostHealthPercentage = (target.MyMaxHP - target.MyCurrentHP) / (float)target.MyMaxHP;
        target.MyStat.passiveAdditionalLostHealthRate = lostHealthPercentage;

    }

    // �ݱ� - ���������� ����
    public void ForbiddenHatred(Skill skill, Character target)
    {
        if (skill == null || target == null || charStat == null) return;

        // ���������� ���ظ� �Դ� "������ ����" ����� �ο� 
        if (target != null && skill != null)
        {
            // ���ֹ����� �����.
            foreach (var option in skill.bonusSpecialOptionList)
            {
                if (option.optionType == OptionType.BUFF)
                {
                    // �� ��ų�� ������ ���������� �ѱ��.
                    continue;
                }

                BuffDebuff buffDebuff = new BuffDebuff();
                buffDebuff.buffName = "HatredCurse";
                buffDebuff.Init(BuffType.DEBUFF, option.effectName, option);
                // �ɼ��� �����ؼ� ����� �Ҵ�
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

    // �ݱ� - �ĸ��ϴ� �г�
    public void ForbiddenAnger(Skill skill, Character target)
    {
        if (skill == null || target == null || charStat == null) return;

        // 1. ġ��Ÿ ���� ���� 
        if (skill != null)
        {
            // �� �ڽ��� ����϶�.. 
            // �̰� ������ �ƴ� ���� ���� �����̴� ���ó�� ��ȭ�Ѵ�.
            // �̷������� ������ ������ �ܺ� ������ ���� ������ �����ϰ� �غ��� ����?�̴�..
            foreach (var option in skill.bonusSpecialOptionList)
            {
                if (option == null || option.optionType == OptionType.DEBUFF)
                {
                    continue;
                }

                // ���ƶ� �� ����..!
                Debug.Log("�нú� ȿ�� : " + option.abilityType + " " + option.value);
                charStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                    option.isPercentage);
            }

            charStat.ApplyOption();
        }
        // 2. ����� ������ �ִ� �����̻��� ���� ��ŭ �߰� ����
        // 1�� : 5 % 2�� 10 % 3�� 15% 
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

            // 1�� �̻� 2�� �̸�
            if(debuffCount >= 1 && debuffCount < 2)
            {
                target.MyStat.passiveAdditionalDamageRate = 0.05f;
            }
            // 2�� �̻� 3�� �̸�
            else if (debuffCount >= 1 && debuffCount < 2)
            {
                target.MyStat.passiveAdditionalDamageRate = 0.1f;
            }
            // 3�� �̻�
            else if (debuffCount >= 3)
            {
                target.MyStat.passiveAdditionalDamageRate = 0.15f;
            }
        }
    }
}
