using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillChain : MonoBehaviour
{
    public int cp;

    public List<Skill> chainSkills = new List<Skill>();

    void ClearSkill()
    {
        chainSkills.Clear();
    }

    void AddSkill(Skill _skill)
    {
        chainSkills.Add(_skill);
    }

    void RemoveSkill(Skill _skill)
    {
        chainSkills.Remove(_skill);
    }

}
