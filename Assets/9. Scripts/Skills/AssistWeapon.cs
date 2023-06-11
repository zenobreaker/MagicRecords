using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistWeapon : MonoBehaviour
{

    bool    isUsed;   // 사용 상태인가 
    float   useTime = 15f; // 사용 시간 
    float   attackDelay = 0.9f; // 공격 딜레이 
    float currentDelay;

    public int attackPower { get; set; } // 웨폰 공격력 

    [SerializeField] GameObject[] weaponsMuzzle;   // 웨폰

    private PlayerControl targetPlayer;

    SkillAttackArea skillPower;
    public PlayerControl TargetPlayer
    {
        private get { return targetPlayer; }
        set { 
            targetPlayer = value;
            isUsed = true;
            currentDelay = attackDelay;
            //bullet = PoolManager.instance.objectPooler.getObject(0);
            //bullet.GetComponent<SkillAttackArea>().skillDamage = attackPower;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            transform.position = targetPlayer.transform.position + Vector3.up * 2.3f;
            transform.rotation = targetPlayer.transform.rotation;
            currentDelay -= Time.deltaTime;

            if (isUsed && targetPlayer != null)
            {
                Debug.Log(targetPlayer.current_Combo_State);
                switch (targetPlayer.current_Combo_State)
                {
                    case ComboState.ATTACK_1:
                    case ComboState.ATTACK_2:
                    case ComboState.ATTACK_3:
                    case ComboState.ATTACK_4:
                        AttackWeapon();
                        break;
                }
            }

            useTime -= Time.deltaTime;

            if (useTime <= 0)
            {
                isUsed = false;
                useTime = 10.0f;
                this.gameObject.SetActive(false);
            }
        }
    }

    

    public void AttackWeapon()
    {
        if (currentDelay <= 0)
        {
            var clone1 = ObjectPooler.SpawnFromPool("Bullet",weaponsMuzzle[0].transform.position,weaponsMuzzle[0].transform.rotation);
            var clone2 = ObjectPooler.SpawnFromPool("Bullet", weaponsMuzzle[1].transform.position, weaponsMuzzle[1].transform.rotation);

            currentDelay = attackDelay;
        }
    }
}
