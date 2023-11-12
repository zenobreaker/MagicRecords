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
    public Sprite sprite; 
    public bool isRunning = false;
    public int buffCount = 0;   // ���� ī��Ʈ

    public bool isRefresh = true;  // ���� ������ �������� üũ

    public bool buffCallFlag = false;
    public float buffCallTime = 0;     // ���� ����� �����ϴ� �ֱ�

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

                        // Ư�� - ������ ���� 
                    case AbilityType.CURSE_HATED:
                        // �� �ʸ��� �������� ���ݷ� ��ŭ ���ظ� �Դ´�. 

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
                        break;
                    case AbilityType.HOLD:
                        break;
                    case AbilityType.SLOW:
                        break;
                    case AbilityType.STURN:
                        break;
                    case AbilityType.ICE:
                        break;

                    // Ư�� - ������ ���� 
                    case AbilityType.CURSE_HATED:
                        // �� �ʸ��� �������� ���ݷ� ��ŭ ���ظ� �Դ´�. 
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
