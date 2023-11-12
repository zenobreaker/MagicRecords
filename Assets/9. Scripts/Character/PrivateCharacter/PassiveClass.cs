using System.Collections.Generic;
using System.Linq;

// ���� ���ϴ� ���� �нú� ��ų���� �����ϴ� Ŭ���� 
// �� ������Ʈ���� ĳ���� = ���� ���������� �ʳ��� ���� ����� �ֱ� ���� �̷� ������ �̸� ����
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

        // ������ ��ų�� �ִ��� �˻� 
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
