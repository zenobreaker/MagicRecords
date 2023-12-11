using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ������(����ź���)��ų �׼��� ����ϴ� Ŭ����
public class MagicShooterSkill : SkillAction
{

    // ��ȭ ��ź 
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
        float resultPower = power * (float)selectedSkill.MyDamage;
        clone1.SetAttackInfo(skillOwn, transform);
        clone1.MyDamage = Mathf.RoundToInt(resultPower);
        yield return null;

        RunSkillFinsihCallback();

    }

    // �ö�� ���� 
    private IEnumerator PlasmaRay(int power = 0)
    {

        float actionTime = selectedSkill.MyCastTime;

        var clone1 = ObjectPooler.SpawnFromPool<SkillAttackArea>("PlasmaRay",
            weaponController.MyWeapon.go_Muzzle.transform);
        clone1.hitCount = selectedSkill.hitCount;
        clone1.damage = power;
        clone1.SetAttackInfo(skillOwn, transform, (float)selectedSkill.MyDamage);

        // todo �Ʒ� ������ ������ �ʿ��ϴ�.?
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

    // ����ź 
    private IEnumerator IceShot(int power = 0)
    {
        float actionTime = selectedSkill.MyCastTime;
        yield return new WaitForSeconds(actionTime);

        var clone1 = ObjectPooler.SpawnFromPool("IceBullet",
            weaponController.MyWeapon.go_Muzzle.transform);

        if (clone1.TryGetComponent<MyBullet>(out var bullet))
        {
            bullet.MyDamage = Mathf.RoundToInt(power);
            bullet.SetAttackInfo(skillOwn, transform, selectedSkill.MyDamage);
            // ����� ���� 
            // ���� ����� ����  - ������ ������ ��ų 
            // ���� ���� �ð� - Sqrt(��ų ���� ��� (�⺻ 1��) * 10)
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
                //Debug.Log(i + "�� ���" + 5 * (i + 1));
                qAngles[i] = Quaternion.Euler(0, 5 * (i + 1) * count, 0);  // 1 : 20 3: 40
            }
            else if ((i + 1) % 2 == 1)
            {
                //Debug.Log(i + "�� Ȧ��" + -5 * (i + 2));
                qAngles[i] = Quaternion.Euler(0, -5 * (i + 2) * count, 0);  // 0 : -20 2 : -40
            }

            //Debug.Log(i + "�� ����" + qAngles[i].y);
            ammoPos[i] = qAngles[i] * weaponController.MyWeapon.go_Muzzle.transform.localPosition;
        }

        yield return new WaitForSeconds(selectedSkill.MyCastTime);


        for (int i = 0; i < qAngles.Length; i++)
        {
            myAnimator.SetTrigger("FinalAttack");
            var clone = ObjectPooler.SpawnFromPool<MyBullet>(
                "SpreadBullet", weaponController.MyWeapon.go_Muzzle.transform.position,
            qAngles[i] * weaponController.MyWeapon.go_Muzzle.transform.rotation);
            clone.SetAttackInfo(skillOwn, transform, selectedSkill.MyDamage);
            clone.MyDamage = power;
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

            clone.SetAttackInfo(skillOwn, transform, selectedSkill.MyDamage);
            clone.MyDamage = power;

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

        clone.MyDamage = power;
        clone.SetAttackInfo(skillOwn, transform, selectedSkill.MyDamage);
        Debug.Log("��ų�� " + selectedSkill.MyDamage);

        yield return null;
        RunSkillFinsihCallback();
    }

    // �Ҹ� ���� 
    private IEnumerator BulletRain(int power = 0)
    {
        yield return new WaitForSeconds(selectedSkill.MyCastTime);

        var weaponPos = weaponController.MyWeapon.go_Muzzle.transform.position;

        // ��ų ���� ��ġ ���� 
        var pos = new Vector3(weaponPos.x, 0, weaponPos.z);
        pos = pos + transform.forward * 3.5f;
        var clone = Instantiate(selectedSkill.MySkillPrefab, pos, Quaternion.identity);

        // clone.GetComponent<BulletRainTrigger>().SetDamage(Mathf.RoundToInt((float)CharStat.instance.c_attack * ((float)selectedSkill.MyDamage / 100)));
        clone.GetComponent<BulletRainTrigger>().ExecutexBulletRain(10,
            skillOwn, transform);

        yield return null;
        RunSkillFinsihCallback();
    }

    // ��ź - �ؾ�..
    private IEnumerator ExtreamBullet(int power = 0)
    {
        yield return new WaitForSeconds(selectedSkill.MyCastTime);

        Vector3 pos = new Vector3(weaponController.MyWeapon.go_Muzzle.transform.position.x, 0, weaponController.MyWeapon.go_Muzzle.transform.position.z);
        pos = pos + transform.forward * 3f;
        var clone = ObjectPooler.SpawnFromPool<SkillAttackArea>("Extream", pos);

        clone.damage = power;
        clone.SetAttackInfo(skillOwn, transform, selectedSkill.MyDamage);

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
