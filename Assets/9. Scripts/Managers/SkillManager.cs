using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance; 

    //private int savedSkillCount = 0;
    //private bool[] isChains = new bool[4];
    private Skill[] skills = new Skill[4];
    private Skill[] chainSkills = new Skill[3];


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    public Skill[] GetSkills()
    {
        return skills; 
    }

    public Skill[] GetChainSkills()
    {
        return chainSkills;
    }

    public void ClearAllSkills()
    {
        skills = null;
        chainSkills = null;
    }


    public  void ClearChianSkillForTarget()
    {
        InfoManual.MyInstance.GetSelectedPlayer().ClearChains();
    }

    // 퀵슬롯에 등록한 스킬 가져오기 
    public  void SetQuickSlostSkill(SkillQuickSlot[] p_skills)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (p_skills[i].GetSkill() != null)
            {
                skills[i] = p_skills[i].GetSkill();
                Debug.Log(skills[i].MyName + i);
            }
        }
    }

    public void SetSkill(Skill p_Skill, int p_Idx)
    {
        skills[p_Idx] = p_Skill;
    }

    public  void SetChainSkill(int p_Target)
    {
        InfoManual.MyInstance.GetSelectedPlayer().SetStartChainSkill(p_Target);
    }

    public  void SetChainSkill(Skill p_Skill, int p_Idx)
    {
        chainSkills[p_Idx] = p_Skill;
    }



    // 등록한 스킬 정보 저장하기 
    public  bool IsHaveSkill(Skill p_skill)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] == p_skill)
                return true;
        }

        return false;
    }

    public  void DeleteSkill(Skill p_skill)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] == p_skill)
                skills[i] = null;
        }
    }

    public  void DeleteChainSkill(Skill p_skill)
    {
        for (int i = 0; i < chainSkills.Length; i++)
        {
            if (chainSkills[i] == p_skill)
                chainSkills[i] = null;
        }
    }
    
    
   
}
