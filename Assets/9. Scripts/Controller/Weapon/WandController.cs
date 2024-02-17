using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Localization.Platform.Android;
using UnityEngine;

// 지팡이 계열을 다루는 컨트롤러
public class WandController : WeaponController
{

    // 나무귀신 전용 기본 평타시 나타나는 오브젝트
    void CreateWandNormalMagicForTreemon()
    {
        if (currentWeapon.go_Muzzle == null) return; 

        var muzzleTransform = currentWeapon.go_Muzzle.transform;

        Vector3 lpos = new Vector3(muzzleTransform.localPosition.x,
            muzzleTransform.localPosition.y, muzzleTransform.localPosition.z);

        //Instantiate(currentWeapon.go_Bullet_Prefab, t_Pos + transform.forward * 3f, Quaternion.identity);

        var tree = ObjectPooler.SpawnFromPool<DisableObjectEvent>("TreeAttack", lpos + muzzleTransform.forward * 3f);
        if (tree != null)
        {
            tree.SetDisableTimer(2.0f);
            AttackArea aa = tree.GetComponentInChildren<AttackArea>();
            if (aa != null)
            {
                aa.SetLayer(targetLayer);
                aa.SetAttackInfo(weaponOwn, gameObject.transform);
            }
        }
    }


    public override void TryNormalMeeleAttack()
    {
        base.TryNormalMeeleAttack();

        if (currentWeapon == null ||
            currentWeapon.attackArea == null ||
            weaponOwn == null)
            return;

        CreateWandNormalMagicForTreemon();
        //currentWeapon.attackArea.SetAttackInfo(weaponOwn, transform);
        //currentWeapon.attackArea.SetOnEnableCollider();
    }

    public override void EndNormalMeeleAttack()
    {
        base.EndNormalMeeleAttack();

        if (currentWeapon == null ||
           currentWeapon.attackArea == null ||
           weaponOwn == null)
            return;

        //currentWeapon.attackArea.SetDisableCollider();
    }
}
