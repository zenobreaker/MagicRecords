using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimController : MonoBehaviour
{
    public WheelerController target;
    public WeaponController weaponController;

    void ActivatedAttackArea()
    {
        // 이 대상이 공격할 수 있는 대상일까?
        if (target != null &&
            target is AttackMonster == true)
        {
            (target as AttackMonster).AttackEableObject(true);
        }
        else if(weaponController != null)
        {
            weaponController.TryNormalMeeleAttack();
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
        else if (weaponController != null)
        {
            weaponController.EndNormalMeeleAttack();
        }
    }

    void CommonAttackEnd()
    {
        if (target != null)
        {
            Debug.Log("공격 마무리");
            target.EndOfAttack();
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
