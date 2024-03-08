using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  �ѱ� �迭 ���⸦ �ַ� �ٷ�� ��Ʈ�ѷ� 
public class GunController : WeaponController
{
    private void CreateBullet()
    {
        // ���Ⱑ ���Ÿ����� �ƴ϶�� �߻������ʴ´�.
        if (weaponType != WeaponType.RANGE || currentWeapon== null)
            return;

        var clone = ObjectPooler.SpawnFromPool<MyBullet>("Bullet", currentWeapon.go_Muzzle.transform);
        // �Ѿ��� ������ ������ �����غ���.
        if(clone != null)
        {
            clone.SetLayer(targetLayer);
            clone.GetLayer();
            clone.SetAttackInfo(weaponOwn, transform);
        }
    }

    void ShotSoundPlay()
    {
        if (currentCombo != ComboState.ATTACK_4)
        {
            SoundManager.instance.PlaySE(currentWeapon.sound_Fire);
        }
        //else if (currentCombo == ComboState.ATTACK_4)
        //{
        //    SoundManager.instance.PlaySE(currentWeapon.sound_Fire2);
        //}

    }

    ////////////////////////////////////////////
    // �ִϸ��̼ǿ� �Ҵ�Ǵ� �̺�Ʈ 
    ////////////////////////////////////////////
    void LaserSound()
    {
        SoundManager.instance.PlaySE("LaserEffect");
        currentWeapon.ps_MuzzleFlash.Play();
    }

    void ShootBullet()
    {
        ShotSoundPlay();
        currentWeapon.ps_MuzzleFlash.Play();
        CreateBullet();
    }


    void ShootFinalBullet()
    {
        SoundManager.instance.PlaySE(currentWeapon.sound_Fire2);
        currentWeapon.ps_MuzzleFlash.Play();
        CreateBullet();
    }
}
