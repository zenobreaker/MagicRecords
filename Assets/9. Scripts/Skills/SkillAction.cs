using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAction : MonoBehaviour
{
    public Character skillOwn; 

    public bool isAction = false;

    [SerializeField] protected SkillChain skillChain = null;
    [SerializeField] protected WeaponController weaponController = null;

    public Animator myAnimator;
    public ActiveSkill selectedSkill;

    public Action callback;

    public bool ActionSkill(ActiveSkill _skill, Character _player, int power)
    {
        if (_player == null) return false;

        if (_skill != null)
        {
            selectedSkill = _skill;
            if (_player.MyCurrentMP >= selectedSkill.SkillCost)
            {
                // 코스트만큼 소비한다. 
                _player.MyCurrentMP -= selectedSkill.SkillCost;
                Debug.Log("스킬 코스트비? " + _player.MyCurrentMP + " = " + selectedSkill.SkillCost);
                isAction = true;
                StartCoroutine(selectedSkill.CallSkillName, _player.MyTotalAttack);
                // 캐스트타임이 어느 정도 존재하면 UI를 보이도록 한다.
                if (selectedSkill.MyCastTime > 1.0f)
                    UIManager.instance.CastSkill(selectedSkill);

                return isAction;
            }
        }
        return false;
    }


    public void RunSkillFinsihCallback()
    {
        isAction = false;

        if (callback != null)
        {
            callback.Invoke();
        }
    }

    public void SetSkillFinishCallback(Action _callback)
    {
        if (_callback != null)
        {
            callback = _callback;
        }
    }


    public void ChianAction()
    {
        StartCoroutine(CoChainAction());
    }

    IEnumerator CoChainAction()
    {
        int i = 0;

        while (i < 4)
        {
            //  PlayerControl.MyInstance.myAnimator.SetFloat("AttackSpeed", CharStat.instance.c_attackSpeed * 1.7f);
            //  myAnimator.SetFloat("AttackSpeed", CharStat.instance.c_attackSpeed * 1.7f);
            //selectedSkill = SkillManager.instance.GetChainSkills()[i++];
            StartCoroutine(selectedSkill.MyName, -selectedSkill.SkillCost);

            yield return new WaitUntil(() => (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f));
        }
        //PlayerControl.MyInstance.myAnimator.SetFloat("AttackSpeed", CharStat.instance.c_attackSpeed);
        //myAnimator.SetFloat("AttackSpeed", CharStat.instance.c_attackSpeed);
    }

  
}
