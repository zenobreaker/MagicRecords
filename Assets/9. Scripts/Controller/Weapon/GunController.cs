using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  총기 계열 무기를 주로 다루는 컨트롤러 
public class GunController : WeaponController
{
    private void CreateBullet()
    {
        // 무기가 원거리형이 아니라면 발사하지않는다.
        if (weaponType != WeaponType.RANGE || currentWeapon== null)
            return;

        var clone = ObjectPooler.SpawnFromPool<MyBullet>("Bullet", currentWeapon.go_Muzzle.transform);
        // 총알은 생성할 때마다 설정해본다.
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
    // 애니메이션에 할당되는 이벤트 
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
