using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// 소위 말하는 직업 패시브 스킬들을 정의하는 클래스 
// 이 프로젝트에선 캐릭터 = 직업 개념이지만 훗날에 직업 기능을 넣기 위해 이런 식으로 이름 지음
public class PassiveClass 
{
    protected CharStat charStat;

    public List<PassiveSkill> equippedPassiveSkills = new List<PassiveSkill>();

    public void InitStat(CharStat stat)
    {
        charStat = stat; 
    }

    public virtual void ApplyPassiveSkillEffects(Character target)
    {

    }

    public void SetPassiveSkill(PassiveSkill skill)
    {
        if (skill == null) return;

        // 기존에 스킬이 있는지 검사 
        var existSkill = equippedPassiveSkills.FirstOrDefault(passive => passive.keycode ==
        skill.keycode);
        if (existSkill != null)
        {
            existSkill = skill;
        }
        else
        {
            equippedPassiveSkills.Add(skill);
        }

    }
}
