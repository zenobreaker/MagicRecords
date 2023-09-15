﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAction : MonoBehaviour
{
    public Character skillOwn; 

    public bool isAction = false;

    [SerializeField] private SkillChain skillChain = null;
    [SerializeField] private WeaponController weaponController = null;

    public Animator myAnimator;
    private ActiveSkill selectedSkill;

    Action callback;

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
                StartCoroutine(selectedSkill.MyName, power);
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

    // 강화 마탄 
    private IEnumerator ReinforcedMagicBullet(float power)
    {
        // exitRotation = FindObjectOfType<PlayerControl>().TargetRotation;

        float actionTime = selectedSkill.MyCastTime;
        yield return new WaitForSeconds(actionTime);

        //GameObject skill = selectedSkill.MySkillPrefab;
        myAnimator.SetTrigger("FinalAttack");
        isAction = true;

        var clone1 = ObjectPooler.SpawnFromPool<Bullet>("Reinforced",
            weaponController.MyWeapon.go_Muzzle.transform);

        clone1.SetAttackInfo(skillOwn, transform, (float)selectedSkill.MyDamage / 100);
        clone1.MyDamage = Mathf.RoundToInt((float)power * ((float)selectedSkill.MyDamage / 100));
        yield return null;

        RunSkillFinsihCallback();

    }

    // 플라즈마 레이 
    private IEnumerator PlasmaRay(int power = 0)
    {

        float actionTime = selectedSkill.MyCastTime;

        var clone1 = ObjectPooler.SpawnFromPool<SkillAttackArea>("PlasmaRay",
            weaponController.MyWeapon.go_Muzzle.transform);
        clone1.hitCount = selectedSkill.hitCount;
        clone1.damage = Mathf.RoundToInt(power * (float)selectedSkill.MyDamage / 100);
        clone1.SetAttackInfo(skillOwn, transform, (float)selectedSkill.MyDamage / 100);

        // todo 아래 로직은 수정이 필요하다.?
        myAnimator.SetFloat("AttackSpeed", 0f);

        if (clone1 != null)
        {
            clone1.SetFinishCallback(() =>
            {
                if (myAnimator != null)
                {
                    myAnimator.SetFloat("AttackSpeed", 1.0f);
                }
                RunSkillFinsihCallback();
            });
        }
        yield return new WaitForSeconds(actionTime);
        myAnimator.SetTrigger("FinalAttack");

        //yield return new WaitForSeconds(2.7f);
        //clone1.SetActive(false);
    }

    // 빙결탄 
    private IEnumerator IceShot(int power = 0)
    {
        float actionTime = selectedSkill.MyCastTime;
        yield return new WaitForSeconds(actionTime);

        var clone1 = ObjectPooler.SpawnFromPool("IceBullet", 
            weaponController.MyWeapon.go_Muzzle.transform);
        if (clone1.TryGetComponent<MyBullet>(out var bullet))
        {
            bullet.MyDamage = Mathf.RoundToInt((float)power * ((float)selectedSkill.MyDamage / 100));
            bullet.SetAttackInfo(skillOwn, transform);
            // 디버프 생성 
            // 빙결 디버프 기준  - 어차피 삭제할 스킬 
            // 빙결 지속 시간 - Sqrt(스킬 레벨 비례 (기본 1초) * 10)
            float applyTime = 1.0f + Mathf.Sqrt(selectedSkill.MySkillLevel * 10.0f);
            bullet.buff = ConditionController.CreateBuffStock(Buff.NONE, Debuff.ICE, false, 0, applyTime);

        }
        //clone1.SetActive(true);

        yield return null;

        RunSkillFinsihCallback();
    }


    private IEnumerator SpreadShot(int power = 0)
    {

        Quaternion[] qAngles = new Quaternion[4];
        Vector3[] ammoPos = new Vector3[4];

        for (int i = 0; i < qAngles.Length; i++)
        {

            float count = i >= qAngles.Length / 2 ? 1.5f : 1;

            if ((i + 1) % 2 == 0)
            {
                //Debug.Log(i + "번 양수" + 5 * (i + 1));
                qAngles[i] = Quaternion.Euler(0, 5 * (i + 1) * count, 0);  // 1 : 20 3: 40
            }
            else if ((i + 1) % 2 == 1)
            {
                //Debug.Log(i + "번 홀수" + -5 * (i + 2));
                qAngles[i] = Quaternion.Euler(0, -5 * (i + 2) * count, 0);  // 0 : -20 2 : -40
            }

            //Debug.Log(i + "번 각도" + qAngles[i].y);
            ammoPos[i] = qAngles[i] * weaponController.MyWeapon.go_Muzzle.transform.localPosition;
        }

        yield return new WaitForSeconds(selectedSkill.MyCastTime);


        for (int i = 0; i < qAngles.Length; i++)
        {
            myAnimator.SetTrigger("FinalAttack");
            var clone = ObjectPooler.SpawnFromPool<MyBullet>(
                "SpreadBullet", weaponController.MyWeapon.go_Muzzle.transform.position,
            qAngles[i] * weaponController.MyWeapon.go_Muzzle.transform.rotation);
            clone.SetAttackInfo(skillOwn, transform, (float)selectedSkill.MyDamage / 100);

            clone.MyDamage = Mathf.RoundToInt((float)power * ((float)selectedSkill.MyDamage / 100));
        }

        yield return null;
        RunSkillFinsihCallback();
    }


    private IEnumerator ConsecutiveShot(int power = 0)
    {

        yield return new WaitForSeconds(selectedSkill.MyCastTime);

        for (int i = 0; i < 5; i++)
        {
            myAnimator.SetTrigger("Attack");
            var clone = ObjectPooler.SpawnFromPool<MyBullet>(
                "Consecutive", weaponController.MyWeapon.go_Muzzle.transform.position,
                weaponController.MyWeapon.go_Muzzle.transform.rotation);

            clone.SetAttackInfo(skillOwn, transform, (float)selectedSkill.MyDamage / 100);
            clone.MyDamage = Mathf.RoundToInt((float)power * ((float)selectedSkill.MyDamage / 100));

            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
        RunSkillFinsihCallback();
    }

    private IEnumerator DeadlySprial(int power = 0)
    {
        yield return new WaitForSeconds(selectedSkill.MyCastTime);

        var clone = ObjectPooler.SpawnFromPool<MyBullet>("SprialBullet", 
            weaponController.MyWeapon.go_Muzzle.transform.position,
             weaponController.MyWeapon.go_Muzzle.transform.rotation);
        clone.SetAttackInfo(skillOwn, transform, (float)selectedSkill.MyDamage / 100);
        clone.MyDamage = Mathf.RoundToInt((float)power * ((float)selectedSkill.MyDamage / 100));
        Debug.Log("스킬댐 " + (float)selectedSkill.MyDamage / 100);

        yield return null;
        RunSkillFinsihCallback();
    }

    // 불릿 레인 
    private IEnumerator BulletRain(int power = 0)
    {
        yield return new WaitForSeconds(selectedSkill.MyCastTime);

        var weaponPos = weaponController.MyWeapon.go_Muzzle.transform.position;

        // 스킬 생성 위치 조정 
        var pos = new Vector3(weaponPos.x, 0, weaponPos.z);
        pos = pos + transform.forward * 3.5f;
        var clone = Instantiate(selectedSkill.MySkillPrefab, pos, Quaternion.identity);

        // clone.GetComponent<BulletRainTrigger>().SetDamage(Mathf.RoundToInt((float)CharStat.instance.c_attack * ((float)selectedSkill.MyDamage / 100)));
        clone.GetComponent<BulletRainTrigger>().ExecutexBulletRain(10, 
            skillOwn, transform);

        yield return null;
        RunSkillFinsihCallback();
    }

    // 마탄 - 극악..
    private IEnumerator ExtreamBullet(int power = 0)
    {
        yield return new WaitForSeconds(selectedSkill.MyCastTime);

        Vector3 pos = new Vector3(weaponController.MyWeapon.go_Muzzle.transform.position.x, 0, weaponController.MyWeapon.go_Muzzle.transform.position.z);
        pos = pos + transform.forward * 3f;
        var clone = ObjectPooler.SpawnFromPool<SkillAttackArea>("Extream", pos);
        clone.SetAttackInfo(skillOwn, transform, (float)selectedSkill.MyDamage / 100);
        //clone.MyDamage = Mathf.RoundToInt((float)power * ((float)selectedSkill.MyDamage / 100));

        yield return null;
        RunSkillFinsihCallback();
    }

    private IEnumerator AssistWeapon(int _power)
    {
        var assistWeapon = Instantiate(selectedSkill.MySkillPrefab);
        assistWeapon.GetComponent<AssistWeapon>().TargetPlayer = this.gameObject.GetComponent<PlayerControl>();
        assistWeapon.GetComponent<AssistWeapon>().attackPower = _power;

        yield return null;
        RunSkillFinsihCallback();
    }
}
