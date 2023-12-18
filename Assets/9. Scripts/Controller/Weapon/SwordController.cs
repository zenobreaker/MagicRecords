using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� �迭 ���⸦ �ٷ�� ��Ʈ�ѷ�
public class SwordController : WeaponController
{
    ////////////////////////////////////////////
    // �ִϸ��̼ǿ� �Ҵ�Ǵ� �̺�Ʈ 
    ////////////////////////////////////////////

    public override void TryNormalMeeleAttack()
    {
        base.TryNormalMeeleAttack();

        if (currentWeapon == null ||
            currentWeapon.attackArea == null ||
            weaponOwn == null)
            return;

        currentWeapon.attackArea.SetAttackInfo(weaponOwn, transform);
        currentWeapon.attackArea.SetOnEnableCollider();
    }

    public override void EndNormalMeeleAttack()
    {
        base.EndNormalMeeleAttack();

        if (currentWeapon == null ||
           currentWeapon.attackArea == null ||
           weaponOwn == null)
            return;

        currentWeapon.attackArea.SetDisableCollider();
    }

    // �� �ֵθ���
    void SwingSword()
    {
        
    }
    
}
