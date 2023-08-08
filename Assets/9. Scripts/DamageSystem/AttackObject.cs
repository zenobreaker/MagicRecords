using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 공격 관련 오브젝트 클래스
public class AttackObject : MonoBehaviour
{
    private Character attackOwn;  // 이 공격 시전자
    public Transform attackOwnTransform;   // 공격 시전자의 transform

    public float damageRate = 1.0f;  
    public int damage;
    public float critRate;
    public float critDamage;

    public float recoveryTime; // 이 공격에 당한 대상이 경직 회복되는데 걸리는 시간
    protected bool isAction = false;
    protected int curHitCount = 0;

    public int hitCount;
    public bool isContiued; // 연속공격인가?

    // 이 공격 기술이 가질 버프들 
    public BuffStock buff = new BuffStock(Buff.NONE, Debuff.NONE, false, 0, 0, false);

    public delegate void Callback();
    protected Callback callback;

    public bool notPool = false;

    public Character AttackOwn
    {
        set { attackOwn = value; }
        get { return attackOwn; }
    }

    public int MyDamage
    {
        set { damage = value; }
        get { return damage; }
    }

    protected void OnDisable()
    {    
        if(notPool == true)
        {
            return; 
        }

        ObjectPooler.ReturnToPool(gameObject);
        if (callback != null)
        {
            callback.Invoke();
        }
    }

    public void SetFinishCallback(Callback _callback)
    {
        callback = _callback;
    }

    // 시전자 정보 저장
    public void SetAttackInfo(Character attacker, Transform trasnform, float damageRate = 1.0f)
    {
        attackOwn = attacker;
        attackOwnTransform = transform; 
        this.damageRate = damageRate;
    }

}
