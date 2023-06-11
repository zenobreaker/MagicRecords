using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRainTrigger : MonoBehaviour
{
    [SerializeField] GameObject go_Base = null;

    int damage;

    public void ExecutexBulletRain(int _count = 10)
    {
        StartCoroutine(BulletRain(_count));
    }

    public void SetDamage(int _damage)
    {
        damage = _damage;
    }

    IEnumerator BulletRain(int count = 10)
    {
        while (count > 0)
        {
             int randPosX = Random.Range(-5, 5);
            int randPosZ = Random.Range(-5, 5);
            Vector3 randPos = new Vector3(go_Base.transform.position.x + randPosX, go_Base.transform.position.y,
                                        go_Base.transform.position.z + randPosZ);
            //Instantiate(go_Bullet, randPos, Quaternion.Euler(new Vector3(-90,0,0)));
            // 총알 생성 
            var bullet = ObjectPooler.SpawnFromPool<MyBullet>("BulletRain", randPos);
            bullet.MyDamage = damage;
           // bullet.GetComponent<SkillAttackArea>().skillDamage = damage;
            //clone.transform.position = randPos;
            bullet.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            //clone.SetActive(true);
            count--;
            yield return new WaitForSeconds(0.3f);
        }

    }
}
