using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 검 계열 무기를 다루는 컨트롤러
public class SwordController : WeaponController
{
    ////////////////////////////////////////////
    // 애니메이션에 할당되는 이벤트 
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

    // 검 휘두르기
    void SwingSword()
    {
        
    }
    
}
