using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : Skill
{
    public bool isUnlocked;

    // ����/����� ���ӽð� ���� ��ġ 
    public float addionalStatusEffectTime;      // ���ӽð� �߰� Ȥ�� ����

    // �����̻� ���� �߰� ����
    public float additionalDamageOnStatusEffect;    // �����̻� �߰� ���� 
    public float additionalDamageOnLostHealth;      // ���� ü�� �߰� ����
    public float additionalDamageCountStatusEffect; // �����̻� ���� �߰� ����
}