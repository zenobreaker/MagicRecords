using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimController : MonoBehaviour
{
    public WheelerController target;
    
    void ActivatedAttackArea()
    {
        //monster.attackRange.enabled = true;
        //if(monster.attackEffect.Length > 0)
        //    monster.attackEffect[0].Play();

            // 이 대상이 공격할 수 있는 대상일까?
        if(target != null && 
            target is AttackMonster == true)
        {
            (target as AttackMonster).AttackEableObject(true);
        }

        SoundManager.instance.PlaySE("Swing");
    }

    void InactivatedAttackArea()
    {
        if (target != null &&
           target is AttackMonster == true)
        {
            (target as AttackMonster).AttackEableObject(false);
        }

    }

    void CommonAttackEnd()
    {
        if (target != null)
        {
            target.isAttacking = false;
        }
    }


    void AttackScratchEnd()
    {

    }

    void BiteEnd()
    {

    }

    void AttackNormalEnd()
    {

    }

}
