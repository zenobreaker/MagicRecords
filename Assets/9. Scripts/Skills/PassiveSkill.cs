using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : Skill
{
    public bool isUnlocked;

    // 버프/디버프 지속시간 관련 수치 
    public float addionalStatusEffectTime;      // 지속시간 추가 혹은 감소

    // 상태이상 관련 추가 피해
    public float additionalDamageOnStatusEffect;    // 상태이상 추가 피해 
    public float additionalDamageOnLostHealth;      // 잃은 체력 추가 피해
    public float additionalDamageCountStatusEffect; // 상태이상 개수 추가 피해
}
