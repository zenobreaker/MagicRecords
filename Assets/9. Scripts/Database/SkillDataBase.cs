using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDataBase : MonoBehaviour
{
    [SerializeField] private Skill[] activeSkills = null;
    [SerializeField] private Skill[] passiveSkills = null;
    [SerializeField] private Skill[] normalActiveSkills = null;
    [SerializeField] private Skill[] normalPassiveSkills = null;

    [SerializeField] private SkillToolTip skillToolTip = null;

    [SerializeField] ActionButton skilbtn = null; 

    // 툴팁 보이기 
    public void ShowSkillToolTip(Skill _skill)
    {
        skillToolTip.ShowToolTip(_skill);
    }

    // 툴팁 숨기기 
    public void HideSkillToolTip()
    {
        skillToolTip.HideToolTip();
    }

    public Skill[] GetActiveSkills()
    {
        return activeSkills;
    }

    public Skill[] GetPassiveSkills()
    {
        return passiveSkills;
    }

    public Skill[] GetNormalActive()
    {
        return normalActiveSkills;
    }

    public Skill[] GetNormalPassive()
    {
        return normalPassiveSkills;
    }

    public void SetSkill(PlayerControl target)
    {
        CharStat tempStat = new CharStat(1, 10, 10, 10, 100, 100, 10);
        Character tempPlayer = new Character();
        tempPlayer.MyStat = tempStat;
        target.MyPlayer = tempPlayer;

        Debug.Log("스킬 정산 " + activeSkills[0].MyName);
        tempPlayer.SetSkill(activeSkills[2], 0, false);
        Debug.Log("스킬 정산 " + tempPlayer.MySkills[0].MyName);
        skilbtn.playerControl = target;
        skilbtn.SetSkill(0);
    }
    
}
