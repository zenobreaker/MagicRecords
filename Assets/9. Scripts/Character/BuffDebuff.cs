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
    BREAK_AROMR,        // �� �ı� (���� ����)
    BREAK_WEAPON,       // ���� �ı� (���ݷ� ����) 
    DOWN_ATTACK_SPEED,  // ���ݼӵ� ����

    BLEED,              // ����  - ���� ������ + ���ݷ� ���� 
    BURN,               // ȭ�� - ���� ������ + ���� ����
    CURSE,              // ���� (��� �������ͽ� ����)
    HOLD,               // �ӹ� - �̵��Ұ� 
    SLOW,               // ��ȭ - ���ݼӵ� / �̵��ӵ� ����
    STURN,              // ���� - ����/�̵� �Ұ� 
    ICE,                // ���� - ������ ����
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
            // ���� ���¿� ���� ȿ�� ����
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
                        // ���ݷ°� ������ 10%�� ���� ��Ų��. 
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
