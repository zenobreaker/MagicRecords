using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Localization.Platform.Android;
using UnityEngine;

// ������ �迭�� �ٷ�� ��Ʈ�ѷ�
public class WandController : WeaponController
{

    // �����ͽ� ���� �⺻ ��Ÿ�� ��Ÿ���� ������Ʈ
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
