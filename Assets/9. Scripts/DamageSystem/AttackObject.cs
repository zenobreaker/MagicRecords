using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���� ���� ������Ʈ Ŭ����
public class AttackObject : MonoBehaviour
{
    private Character attackOwn;  // �� ���� ������
    public Transform attackOwnTransform;   // ���� �������� transform

    public float damageRate = 1.0f;  
    public int damage;
    public float critRate;
    public float critDamage;

    public float recoveryTime; // �� ���ݿ� ���� ����� ���� ȸ���Ǵµ� �ɸ��� �ð�
    protected bool isAction = false;
    protected int curHitCount = 0;

    public int hitCount;
    public bool isContiued; // ���Ӱ����ΰ�?

    // �� ���� ����� ���� ������ 
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

    // ������ ���� ����
    public void SetAttackInfo(Character attacker, Transform trasnform, float damageRate = 1.0f)
    {
        attackOwn = attacker;
        attackOwnTransform = transform; 
        this.damageRate = damageRate;
    }

}
