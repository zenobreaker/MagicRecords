using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.TextCore.Text;


// �⺻ ĳ���� : ������
// �ݱ� -  ���� �����
public class CursedTurtle : Character
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
                    ForbiddenMagic(skill);
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
                    ForbiddenGrief(skill);
                    break;
                // �ݱ� - ���������� ����
                case "forbidden-Hatred":
                    ForbiddenHatred(skill);
                    break;
                // �ݱ� - �ĸ��ϴ� �г�
                case "forbidden-Anger":
                    ForbiddenAnger(skill);
                    break;
                // �������κ����� ����
                case "curseBeyond": break;
            }

        }

      
    }

    public override void ApplyPassiveSkillEffectsByWC(WheelerController wheelerController)
    {
        base.ApplyPassiveSkillEffectsByWC(wheelerController);

        foreach (var skill in equippedPassiveSkills)
        {
            if (skill == null || skill.isUnlocked == false) continue;

            switch (skill.keycode)
            {
                // 1. ������ ���� 
                case "forbiddenMagic":
                    ForbiddenMagic(skill, wheelerController);
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
                    ForbiddenGrief(skill, wheelerController);
                    break;
                // �ݱ� - ���������� ����
                case "forbidden-Hatred":
                    ForbiddenHatred(skill, wheelerController);
                    break;
                // �ݱ� - �ĸ��ϴ� �г�
                case "forbidden-Anger":
                    ForbiddenAnger(skill, wheelerController);
                    break;
                // �������κ����� ����
                case "curseBeyond": break;
            }


        }
    }



    // ������ ���� - ����
    private void ForbiddenMagic(Skill skill, WheelerController target = null)
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
                // todo �ӽ÷� �нú� �������� �־��ش�. 
                buffDebuff.sprite = skill.MyIcon;
                buffDebuff.Init(BuffType.DEBUFF, option.effectName, option);
                target.AddBuffDebuff(buffDebuff);
            }
                    
        }
        // ���ݷ°� ġ��Ÿ ��� 
        else if(skill != null )
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
                MyStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                    option.isPercentage);
            }

            MyStat.ApplyOption();
        }
    }


    // ��ź ���� 
    public void RenovateBullet(Skill skill)
    {
        if (skill == null) return;
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
            MyStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                option.isPercentage);
        }

        MyStat.ApplyOption();

    }

    // �ݱ� - ����ϴ� ��ź
    public void ForbiddenGrief(Skill skill, WheelerController target = null)
    {
        if (skill == null || target == null || target.MyPlayer == null) return;

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
        float lostHealthPercentage = (target.MyPlayer.MyMaxHP - target.MyPlayer.MyCurrentHP) / (float)target.MyPlayer.MyMaxHP;

        // �ִ� 10/20/30% ����
        float maxAdditionalDamagePercentage = 0.1f * skill.MySkillLevel; 

        target.MyPlayer.MyStat.passiveAdditionalLostHealthRate = maxAdditionalDamagePercentage * lostHealthPercentage;

    }

    // �ݱ� - ���������� ����
    public void ForbiddenHatred(Skill skill, WheelerController target = null)
    {
        if (skill == null || target == null || MyStat == null) return;

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
                buffDebuff.Init(BuffType.DEBUFF, option.effectName, option.Clone());
                // �ɼ��� �����ؼ� ����� �Ҵ�
                if(buffDebuff.specialOption != null)
                {
                    float rate = 0.03f;
                    buffDebuff.buffCallFlag = true;
                    buffDebuff.buffCallTime = 1.0f;
                    buffDebuff.isRefresh = false;   // �� ������ ���� �Ұ�
                    buffDebuff.specialOption.duration = 3;
                    buffDebuff.specialOption.value = 1.0f + Mathf.Round(MyStat.totalATK * rate);
                    buffDebuff.specialOption.SetCoolTime();
                    // todo �ӽ÷� �нú� �������� �־��ش�. 
                    buffDebuff.sprite = skill.MyIcon;
                }
                target.AddBuffDebuff(buffDebuff);
            }
        }
    }

    // �ݱ� - �ĸ��ϴ� �г�
    public void ForbiddenAnger(Skill skill, WheelerController target = null)
    {
        if (skill == null || target == null || MyStat == null
            || target.MyPlayer == null) return;

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
                MyStat.extraStat.ApplyOptionExtraStat(option.abilityType, option.value,
                    option.isPercentage);
            }

            MyStat.ApplyOption();
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
                target.MyPlayer.MyStat.passiveAdditionalDamageRate = 0.05f;
            }
            // 2�� �̻� 3�� �̸�
            else if (debuffCount >= 1 && debuffCount < 2)
            {
                target.MyPlayer.MyStat.passiveAdditionalDamageRate = 0.1f;
            }
            // 3�� �̻�
            else if (debuffCount >= 3)
            {
                target.MyPlayer.MyStat.passiveAdditionalDamageRate = 0.15f;
            }
        }
    }
}
