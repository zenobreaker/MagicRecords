using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���� ���� ������Ʈ Ŭ����
public class AttackObject : MonoBehaviour
{
    private Character attackOwn;  // �� ���� ������
    public Transform attackOwnTransform;   // ���� �������� transform
    
    public LayerMask targetLayer;   // �������� ����� ���̾�

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

    public void SetLayer(string[] layerNames)
    {
        foreach (var name in layerNames)
        {
            targetLayer |= LayerMask.NameToLayer(name);
        }

        //targetLayer = targetLayerMask;
    }

    // PRRIAVET METHOD : ���� ��Ʈ�� �˻��ϴ� �޼ҵ�
    private int CountSetBits(LayerMask mask)
    {

        int count = 0; 
        for (int i = 0; i < 32; i++)
        {
            if (((1 << i) & mask )!= 0)
                count++; 
        }

        return count; 
    }

    public void SetLayer(string name)
    {
        targetLayer |= (1 << LayerMask.NameToLayer(name));
    }
    
    public void SetLayer(LayerMask layer)
    {
        // ���� ���̾� ������ ������ üũ�Ѵ�.
        var count = CountSetBits(layer);
        if ( count > 1)
        {
            for(int i = 0; i < 32; i++)
            {
                if(( (1 << i ) & layer ) != 0)
                {
                    string layerName = LayerMask.LayerToName(i);
                    targetLayer |= (1 <<  LayerMask.NameToLayer(layerName));
                }
            }
        }
        else
        {
            targetLayer |= (1 << layer);
        }
    }

    public LayerMask GetLayer()
    {
        Debug.Log(LayerMask.LayerToName(targetLayer));
        return targetLayer;
    }

    public void SetIgnoreLayer(string layerName)
    {
        int myLayer = this.transform.gameObject.layer;
        Physics.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(layerName), true);
    }

    public void SetFinishCallback(Callback _callback)
    {
        callback = _callback;
    }

    // ������ ���� ����
    public void SetAttackInfo(Character attacker, Transform paraTrasnform, float damageRate = 1.0f)
    {
        attackOwn = attacker;
        attackOwnTransform = paraTrasnform; 
        this.damageRate = damageRate;
    }

    public void SetDisableTimer(float delay)
    {
        StartCoroutine(DisableObjectAfterDelay(delay));
    }

    IEnumerator DisableObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

}
